using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace AmobaProject_Vision_Processor
{
    /// <summary>
    /// Az osztály a tábla ferdén való felismeréséért felelős.
    /// </summary>
    public class CrossCageFinder
    {
        private float avgSize; //Mezők nagysága.
        private float avgAngle; //A tábla dőlésszöge a mezők átlagából számítva.
        private PointF avgCenter; //Mezők alkotta pálya középpontja.

        /// <summary>
        /// A kapott képen található négyzetháló keretét keresi.
        /// </summary>
        /// <param name="img">A kép, amin keressük a tábla keretét.</param>
        /// <returns>A detektált négyzetháló keretét adja vissza.</returns>
        public DetectedField GeneralField(Image<Bgr, Byte> img)
        {
            List<MCvBox2D> allBoxes = RealBoxes(Boxes(img), 400);
            List<MCvBox2D> uniqueBoxes = UniqueBoxes(allBoxes);
            List<MCvBox2D> relevantBoxes = RelevantBoxes(uniqueBoxes);

            //Beállítunk néhány táblára jellenző értéket, melyet később felhasználunk.
            avgCenter = AvgCenter(relevantBoxes);
            avgAngle = AvgAngle(relevantBoxes);
            avgSize = AvgSize(relevantBoxes);

            MCvBox2D frameBox = new MCvBox2D();
            //Ha megfelelő szögben áll a pálya, akkor egy egyszerűbb metódus fut le.
            if (Math.Abs(avgAngle) < 3 || Math.Abs(avgAngle) > 87)
                frameBox = StraightFrame(avgSize, relevantBoxes);
            else //Ferde négyzethálót kell keresni.
                frameBox = CrossFrame(avgSize, relevantBoxes);

            return new DetectedField(frameBox, avgSize);
        }

        #region FindFrame
        /// <summary>
        /// Megkeresi openCV segítségével a paraméterként kapott képen az összes lehetséges négyszöget.
        /// </summary>
        /// <param name="img">A kép, melyen négyszögeket keresünk.</param>
        /// <returns>Az összes detektált négyzetet adja vissza.</returns>
        private List<MCvBox2D> Boxes(Image<Bgr, Byte> img)
        {   
            List<MCvBox2D> boxList = new List<MCvBox2D>();

            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();
            Image<Gray, Byte> cannyEdges = img.Canny(100.0, 60.0);

            Contour<Point> contours;
            using (MemStorage storage = new MemStorage())
                for (contours = cannyEdges.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage); contours != null; contours = contours.HNext)
                {
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    bool isRectangle = true;
                    Point[] pts = currentContour.ToArray();
                    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                    for (int i = 0; i < edges.Length; i++)
                    {
                        double angle = Math.Abs(
                        edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                        if (angle < 80 || angle > 100)
                        {
                            isRectangle = false;
                            break;
                        }
                    }

                    if (isRectangle) boxList.Add(currentContour.GetMinAreaRect());
                }

            return boxList;
        }

        /// <summary>
        /// A nagyon kicsi négyzeteket kiveszi a listából.
        /// </summary>
        /// <param name="boxList">A szűrni kívánt lista.</param>
        /// <param name="minimumSize">Mimimális nagyság, melynél kisebb négyszögeket nem tartunk meg.</param>
        /// <returns>A túl kicsi négyzetek nélküli listát adja vissza.</returns>
        private List<MCvBox2D> RealBoxes(List<MCvBox2D> boxList, int minimumSize)
        {   
            List<MCvBox2D> realBoxes = new List<MCvBox2D>();

            foreach (MCvBox2D box in boxList)
                if (box.size.Width * box.size.Height > minimumSize)
                    realBoxes.Add(box);

            return realBoxes;
        }

        /// <summary>
        /// Azon négyzeteket szűri, melyeknek center koordinátája nagyon közel esik egymáshoz.
        /// Ez esetben csak egyetlen ilyen négyszöget tart meg, majd ad vissza egy listában.
        /// </summary>
        /// <param name="boxList">A redundáns négyzetekkel teli lista.</param>
        /// <returns>A visszatérési érték a redundanciamentes négyzetek listája.</returns>
        private List<MCvBox2D> UniqueBoxes(List<MCvBox2D> boxList)
        {   
            List<MCvBox2D> uniqueBoxes = new List<MCvBox2D>();

            for (int i = 0; i < boxList.Count; i++)
            {
                int j = 0;

                bool nincs = false;
                while (nincs == false && j < uniqueBoxes.Count)
                {
                    Point p1 = new Point((int)boxList[i].center.X, (int)boxList[i].center.Y);
                    Point p2 = new Point((int)uniqueBoxes[j].center.X, (int)uniqueBoxes[j].center.Y);
                    nincs = isTheSamePoint(p1, p2, 20);
                    j++;
                }

                if (!nincs)
                    uniqueBoxes.Add(boxList[i]);
            }

            return uniqueBoxes;
        }

        /// <summary>
        /// A listában paraméterként kapott négyzeteket méret szerint kategorizáljuk.
        /// Azon kategória négyzeteit kapjuk kissza, ahol a legtöbb négyzet szerepelt.
        /// </summary>
        /// <param name="boxList">Kategorizálni kívánt négyzetek.</param>
        /// <returns>A legtöbb négyzetet tartalmazó kategória listáját adja vissza.</returns>
        private List<MCvBox2D> RelevantBoxes(List<MCvBox2D> boxList)
        {   
            List<MCvBox2D> relevantBoxes = new List<MCvBox2D>();

            
            Dictionary<Size, List<MCvBox2D>> categories = new Dictionary<Size, List<MCvBox2D>>(); //A kategóriák ebbe kerülnek
            foreach (MCvBox2D box in boxList)
            {
                Size result = new Size(0, 0); //Kezdeti kulcsértéknek 0,0 értéket definiálunk.
                Size current = new Size((int)box.size.Width, (int)box.size.Height);
                if (isRect(current, 15))
                {   //If it's a square
                    foreach (Size key in categories.Keys) //Megvizsgáljuk az eddig felvett kategóriákat.
                    {
                        if (isTheSameSize(key, current, 7)) //Ha a kategóriák között szerepelt már hasonló, akkor a kulcs értéket eltároljuk, hiszen oda fog kerülni.
                        {
                            result = key;
                        }
                    }
                }

                if (result.Width > 0 && result.Height > 0) //Ha a 0,0 érték változott, akkor volt már ilyen kategóriánk.
                    categories[result].Add(box);
                else
                {   
                    if (!categories.Keys.Contains(current))
                    {
                        categories.Add(current, new List<MCvBox2D>()); //Ellenben fel kell vennünk egy új kategróiát.
                        categories[current].Add(box); //Majd abba besorolni a négyszöget.
                    }
                    else
                        categories[current].Add(box);
                }
            }

            int maxCount = 0;
            foreach (Size key in categories.Keys) //Megvizsgáljuk, melyik kategóriában van a legtöbb négyzet.
            {
                if (categories[key].Count > maxCount)
                {
                    maxCount = categories[key].Count;
                    relevantBoxes = categories[key];
                }
            }

            return relevantBoxes;
        }

        /// <summary>
        /// Megvizsgálja, hogy a paraméterként kapott első pont a paraméterként kapott második pont threshold méretű sugarában található-e.
        /// </summary>
        /// <param name="innerPoint">A hasonlítandó pont.</param>
        /// <param name="outerPoint">A pont, amihez hasonlítunk.</param>
        /// <param name="threshold">Küszöbérték, amin belül még egyezőnek tekintjük a két pontot.</param>
        /// <returns>A képt pont egyezősége esetén TRUE, ellenben FALSE értékkel tér vissza.</returns>
        private bool isTheSamePoint(Point innerPoint, Point outerPoint, int threshold)
        {   
            if ((innerPoint.X < outerPoint.X + threshold &&
                 innerPoint.X > outerPoint.X - threshold) &&
                 innerPoint.Y < outerPoint.Y + threshold &&
                 innerPoint.Y > outerPoint.Y - threshold)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Két méretről eldönti, hogy egyező méretűek-e.
        /// </summary>
        /// <param name="size1">Hasonlítandó méret.</param>
        /// <param name="size2">Amihez hasonlítjuk.</param>
        /// <param name="threshold">Küszöbérték, amin belül még hasonlónak tekintendő.</param>
        /// <returns>A két méret egyezése esetén TRUE, ellenben FALSE értékkel tér vissza.</returns>
        private bool isTheSameSize(Size size1, Size size2, int threshold)
        {
            if (Math.Abs(size1.Width - size2.Width) < threshold && Math.Abs(size1.Height - size2.Height) < threshold)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Eldönti hogy a paraméterként kapott méret szélessége és magassága nagyjából megegyezik e.
        /// </summary>
        /// <param name="size">Vizsgálandó méret.</param>
        /// <param name="threshold">Küszöbérték, amin belül még négyzetnek tekinjük.</param>
        /// <returns>Ha a kapott Size szélessége, hosszúsága threshold értéken belül megegyezik, akkor TRUE eredényt szolgál.</returns>
        private bool isRect(Size size, int threshold)
        {   //Is it a Rect...? :)
            if (Math.Abs(size.Width - size.Height) < threshold)
                return true;
            else
                return false;
        }
        #endregion FindFrame
        
        #region MakeField
        /// <summary>
        /// Visszaad egy közel 0 fokos négyzetet. Próbál illeszkedni az azt alkotó négyzetek dőlésszögéhez.
        /// </summary>
        /// <param name="size">A négyzetet alkotó mezők átlagos nagysága.</param>
        /// <param name="boxList">A négyzetet alkotó mezők.</param>
        /// <returns>A paraméterek alapján generált négyzettel tér vissza.</returns>
        private MCvBox2D StraightFrame(float size, List<MCvBox2D> boxList)
        {   
            float left = boxList[0].center.X;
            float right = left; 
            float top = boxList[0].center.Y; 
            float bottom = top;

            foreach (MCvBox2D box in boxList) //Megkeresi a pálya szélső értékeit (teleje, alja, jobb és bal széle)
            {
                float bcx = box.center.X;
                float bcy = box.center.Y;

                if (bcx < left)
                    left = bcx;
                else
                    if (bcx > right)
                        right = bcx;

                if (bcy < top)
                    top = bcy;
                else
                    if (bcy > bottom)
                        bottom = bcy;
            }

            float width = (float)((bottom - top + size)); //Tábla szélessége.
            float height = (float)((right - left + size)); //Tábla magassága.

            //A 0 közeli és 90 fok közeli négyzeteknél néha problémás a kirajzolás.
            if (Math.Abs(avgAngle) > 88)
                return new MCvBox2D(avgCenter, new SizeF(width, height), avgAngle);
            else
                return new MCvBox2D(avgCenter, new SizeF(height, width), avgAngle);

        }

        /// <summary>
        /// Visszaad négyzetet, ami a paraméterként kapott négyzetek kerete.
        /// </summary>
        /// <param name="size">A lista négyzeteinek átlagos mérete.</param>
        /// <param name="boxList">A négyzetek listája, melynek a keretét keresi.</param>
        /// <returns>A paraméterek alapján generált négyzettel tér vissza.</returns>
        private MCvBox2D CrossFrame(float size, List<MCvBox2D> boxList)
        {   
            List<MyLine> lineList = LinesWithHough(boxList); //Négyzetek középpontjaira felírható egyeneseket keres.

            List<MyLine> horizontalLine = SuitableLines(avgAngle, lineList, true); //A négyzet szögével horizontálisan egyező dőlésszögű egyeneseket keres.
            List<MyLine> verticalLine = SuitableLines(avgAngle, lineList, false); //A négyzet szögével vertikálisan egyező dőlésszögű egyeneseket keres.
            float bigBoxWidth = LongestLine(horizontalLine) + size; //A keret szélessége a leghosszabb horizontális vonal + egy mező szélesség, mivel center pozíciókkal számoltunk.
            float bigBoxHeight = LongestLine(verticalLine) + size;

            if (Math.Abs(avgAngle) < 45)
                return new MCvBox2D(AvgCenter(boxList), new SizeF(bigBoxWidth, bigBoxHeight), avgAngle);
            else
                return new MCvBox2D(AvgCenter(boxList), new SizeF(bigBoxHeight, bigBoxWidth), avgAngle);
        }

        /// <summary>
        /// A paraméterül kapott vonalakból a leghosszabbat keresi.
        /// </summary>
        /// <param name="lines">Vonalak listája.</param>
        /// <returns>A leghosszabb vonal a listából.</returns>
        private float LongestLine(List<MyLine> lines)
        {   
            float max = 0;
            foreach (MyLine l in lines)
            {
                if (l.Length() > max)
                    max = l.Length();
            }
            return max;
        }

        /// <summary>
        /// Hough transzformáció segítségével, a paraméterül kapott négyzetek középpontjaira felírható egyeneseket keres.
        /// </summary>
        /// <param name="boxList">Négyzetek listája.</param>
        /// <returns>Egyenesek listája.</returns>
        private List<MyLine> LinesWithHough(List<MCvBox2D> boxList)
        {   
            Dictionary<PointF, PointsOfLine> allSinusoid = new Dictionary<PointF, PointsOfLine>();
            List<MyLine> lines = new List<MyLine>();

            foreach (MCvBox2D box in boxList) //Szinuszoidokat készít a négyzetek középpontja alapján.
            {
                for (int i = 0; i < 180; i++)
                {
                    float inX = box.center.X;
                    float inY = box.center.Y;

                    int r = (int)(inX * Math.Cos(toRadian(i)) +
                                  inY * Math.Sin(toRadian(i)));

                    PointF keyCoord = new PointF(i, r);
                    PointF matchF = new PointF(0, 0);
                    bool matchB = false;

                    foreach (PointF key in allSinusoid.Keys)
                    {
                        if ((key.X + 10 > keyCoord.X && key.X - 10 < keyCoord.X) &&
                            (key.Y + 10 > keyCoord.Y && key.Y - 10 < keyCoord.Y))
                        {
                            matchB = true;
                            matchF = key;
                        }
                    }

                    if (matchB)
                    {
                        allSinusoid[matchF].addPoint(new PointF(inX, inY));
                    }
                    else
                    {
                        allSinusoid.Add(keyCoord, new PointsOfLine());
                        allSinusoid[keyCoord].addPoint(new PointF(inX, inY));
                    }

                }
            }

            foreach (PointF key in allSinusoid.Keys) //Azon kulcs értékekre van szükségünk, ahol több mint egy elem található.
            {
                if (allSinusoid[key].Count() > 1)
                {
                    PointF a = allSinusoid[key].getA();
                    PointF b = allSinusoid[key].getB();

                    if (a.X != b.X || a.Y != b.Y)
                    {
                        bool duplicated = false;
                        foreach (MyLine line in lines) //Kerekítésből fakadóan vannak redundáns vonalak, ezeket el kell távolítani.
                        {
                            PointF c = line.pointA; //Hasonlítandó elemek.
                            PointF d = line.pointB;
                            if (((int)a.X == (int)c.X && (int)a.Y == (int)c.Y) &&
                                ((int)b.X == (int)d.X && (int)b.Y == (int)d.Y))
                                duplicated = true;
                        }

                        if (!duplicated)
                            lines.Add(new MyLine(allSinusoid[key].getA(), allSinusoid[key].getB()));
                    }
                }
            }

            return lines;
        }

        /// <summary>
        /// A paraméterek alapján a megfelelő egyeneseket keresi.
        /// </summary>
        /// <param name="boxDegree">Négyzet dőlésszöge.</param>
        /// <param name="lineList">Vonalak listája.</param>
        /// <param name="horizontal">Ha horizontális vonal kell, akkor TRUE, ellenben FALSE értékkel hívandó.</param>
        /// <returns>A paramétereknek megfelelő egyenesek listáját adja eredményük.</returns>
        private List<MyLine> SuitableLines(float boxDegree, List<MyLine> lineList, bool horizontal)
        {   
            List<MyLine> result = new List<MyLine>();

            foreach (MyLine line in lineList)
            {
                float bd; //boxDegree
                float ld; //lineDegree
                if (horizontal)
                {
                    bd = (boxDegree + 90) * -1;
                    ld = line.Degree(true);
                }
                else
                {
                    bd = boxDegree;
                    ld = line.Degree(false);
                }

                if (bd + 2 > ld && bd - 2 < ld)
                    result.Add(line);
            }

            return result;
        }
        #endregion MakeField

        #region SetAvgProperties
        private float AvgSize(List<MCvBox2D> boxList)
        {   //returns with avg size of boxes
            float size = 0;
            foreach (MCvBox2D box in boxList)
            {
                size += box.size.Width;
            }

            return size / (float)boxList.Count;
        }

        private float AvgAngle(List<MCvBox2D> boxes)
        {   /* A négyzetek dőlésszöge 0-90 fok között lehet.
             * A 0 fok közeli négyzeteket néha 88-89 fokosnak ismeri fel az OpenCV.
             * Ezért nem lehet csak szimplán átlagolni őket.
             */
            float result = 0;
            int smaller = 0; //smaller than 45 degree
            int bigger = 0;  //bigger than 45 degree
            foreach (MCvBox2D box in boxes)
            {
                if (Math.Abs(box.angle) > 45)
                {
                    result += -1 * (90 + box.angle);
                    bigger++;
                }
                else
                {
                    result += box.angle;
                    smaller++;
                }
            }

            if (smaller < bigger)
            {
                if (Math.Abs(result / boxes.Count) > 45)
                    return result / boxes.Count;
                else
                    return (90 + result / boxes.Count) * -1;
            }
            else
            {
                if (Math.Abs(result / boxes.Count) > 45)
                    return (90 + result / boxes.Count) * -1;
                else
                    return result / boxes.Count;
            }
        }

        /// <summary>
        /// Az kapott négyzetekből kiszámolja, hogy hol van az őket határoló keret középpontja.
        /// </summary>
        /// <param name="boxes">Négyzetek listája.</param>
        /// <returns>Keret középpontja.</returns>
        private PointF AvgCenter(List<MCvBox2D> boxes)
        {   
            float maxDiff = 0;
            float curDiff = 0; //current diff
            PointF a = new PointF(0, 0);
            PointF b = new PointF(0, 0);

            for (int i = 0; i < boxes.Count; i++)
                for (int j = 0; j < boxes.Count; j++)
                {
                    curDiff = (float)Math.Sqrt(Math.Pow(boxes[i].center.X - boxes[j].center.X, 2) +
                                               Math.Pow(boxes[i].center.Y - boxes[j].center.Y, 2));

                    if (curDiff > maxDiff)
                    {
                        a = boxes[i].center;
                        b = boxes[j].center;
                        maxDiff = curDiff;
                    }
                }

            if ((a.X + b.X) / 2 < 1 && (a.Y + b.Y) / 2 < 1 && boxes.Count > 0)
                return boxes[0].center;
            return new PointF((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        private double toRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        #endregion SetAvgProperties
    }

    /// <summary>
    /// A detektált pálya matamétereit reprezentáló osztály.
    /// </summary>
    public class DetectedField
    {
        public MCvBox2D frameBox { get; set; } //Keret
        public int rowCount { get; set; } //Sorok száma
        public int colCount { get; set; } //Oszlopok száma
        public float avgBoxSize { get; set; } //Mezők mérete

        public DetectedField(MCvBox2D frameBox, float avgFieldSize) //Konstruktor
        {
            this.frameBox = frameBox;
            this.avgBoxSize = avgFieldSize;
            if (Math.Abs(frameBox.angle) < 45)
            {
                rowCount = (int)(frameBox.size.Width * 1.05 / avgFieldSize);
                colCount = (int)(frameBox.size.Height * 1.05 / avgFieldSize);
            }
            else
            {
                colCount = (int)(frameBox.size.Width * 1.05 / avgFieldSize);
                rowCount = (int)(frameBox.size.Height * 1.05 / avgFieldSize);
            }
        }

        /// <summary>
        /// Összehasonlít két DetectedField objektumot.
        /// </summary>
        /// <param name="obj">A hasonlítandó objektum.</param>
        /// <returns>A hasonlítás eredménye.</returns>
        public bool isTheSame(Object obj)
        {
            DetectedField field = obj as DetectedField;

            if (field == null)
            {
                return false;
            }
            else
                if (isTheSamePoint(new Point((int)field.frameBox.center.X, (int)field.frameBox.center.Y),
                                   new Point((int)field.frameBox.center.X, (int)field.frameBox.center.Y), 10)
                    && field.rowCount == this.rowCount
                    && field.colCount == this.colCount)
                //ha a center pozíciója, sor és oszlopszám is egyezik, akkor egyezőnek tekintem a két objektumot
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }

        /// <summary>
        /// Megvizsgálja, hogy a paraméterként kapott első pont a paraméterként kapott második pont threshold méretű sugarában található-e.
        /// </summary>
        /// <param name="innerPoint">A hasonlítandó pont.</param>
        /// <param name="outerPoint">A pont, amihez hasonlítunk.</param>
        /// <param name="threshold">Küszöbérték, amin belül még egyezőnek tekintjük a két pontot.</param>
        /// <returns>A képt pont egyezősége esetén TRUE, ellenben FALSE értékkel tér vissza.</returns>
        private bool isTheSamePoint(Point innerPoint, Point outerPoint, int threshold)
        {
            if ((innerPoint.X < outerPoint.X + threshold &&
                 innerPoint.X > outerPoint.X - threshold) &&
                 innerPoint.Y < outerPoint.Y + threshold &&
                 innerPoint.Y > outerPoint.Y - threshold)
                return true;
            else
                return false;
        }

        public String toString()
        {
            return "A négyzet: " + colCount + " széles, és " + rowCount + " magas.";
        }
    }

    /// <summary>
    /// A detektált bábuk leírását szolgáló osztály.
    /// </summary>
    public class DetectedPuppet
    {
        public Rectangle puppetRect { get; set; } //A bábu térbeli kiterjedése.
        public String puppetValue { get; set; } //A bábu értéke (X, O)
        public Point center { get; set; } //A bábú középpontja. 

        public DetectedPuppet(Rectangle rect, String XO, Point center)
        {
            this.puppetRect = rect;
            this.puppetValue = XO;
            this.center = center;
        }

        public String toString()
        {
            return "Érték: " + puppetValue + ", rect: " + puppetRect + ", center: " + center;
        }
    }

    /// <summary>
    /// Az osztály vonalak reprezentálására szolgál.
    /// Mindössze a kezdő és végpontját, illetve az ebből kapott hosszát tartalmazaz.
    /// </summary>
    public class MyLine
    {
        public PointF pointA { get; set; } //Vonal kezdőpontja
        public PointF pointB { get; set; } //Vonal végpontja
        private float length; //Vonal hossza

        public MyLine(PointF pointA, PointF pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
            length = Length();
        }

        /// <summary>
        /// A vonal dőlésszögét határozza meg.
        /// </summary>
        /// <param name="horizontal">Horivontális vonal esetén TRUE.</param>
        /// <returns>A vonal dőlésszögét adja eredményül.</returns>
        public float Degree(bool horizontal)
        {
            float width;
            float height;
            if (horizontal)
            {
                width = Math.Abs(pointB.Y - pointA.Y);
                height = Math.Abs(pointB.X - pointA.X);
            }
            else
            {
                width = Math.Abs(pointB.X - pointA.X);
                height = Math.Abs(pointB.Y - pointA.Y);
            }

            float degree = 0;

            if (height > width)
            {
                degree = -1 * (float)(toDegree(Math.Atan(width / height)));
                if (!(pointA.X < pointB.X && pointA.Y < pointB.Y))
                    degree = -1 * (degree + 90);
            }

            return degree;
        }

        public float Length()
        {
            return (float)Math.Sqrt(Math.Pow(pointA.X - pointB.X, 2) +
                          Math.Pow(pointA.Y - pointB.Y, 2));
        }

        private double toDegree(double angle)
        {
            return 180.0 * angle / Math.PI;
        }
    }

    /// <summary>
    /// Vonalat reprezentáló pontokat tárolja objektumként.
    /// </summary>
    public class PointsOfLine
    {
        private List<PointF> points;
        private float min;
        private float max;

        public PointsOfLine()
        {
            points = new List<PointF>();
        }

        public void addPoint(PointF currentPoint)
        {
            points.Add(currentPoint);
        }

        /// <summary>
        /// Visszaadja az egynes egyik végpontját,
        /// amely vagy a bal alsó, vagy a bal felső.
        /// </summary>
        /// <returns>PointF</returns>
        public PointF getA()
        {
            if (isWide(out min, out max) == true)
                return getThis(true, min);
            else
                return getThis(false, min);
        }

        /// <summary>
        /// Visszaadja az egynes másik végpontját,
        /// amely vagy a jobb alsó, vagy a jobb felső.
        /// </summary>
        /// <returns>PointF</returns>
        public PointF getB()
        {
            if (isWide(out min, out max) == true)
                return getThis(true, max);
            else
                return getThis(false, max);
        }

        /// <summary>
        /// A ponttok közül megkeresi azt, ahol az X vagy Y
        /// koordináta megegyezik a paraméterül kapott value értékkel.
        /// </summary>
        /// <param name="x">Ha X-hez kell hasonlítani: true, Y esetén: false.</param>
        /// <param name="value">Az érték, amivel egyeznie kell.</param>
        /// <returns>PointF</returns>
        private PointF getThis(bool x, float value)
        {
            foreach (PointF point in points)
            {
                if (x)
                {
                    if (point.X == value)
                        return point;
                }
                else
                    if (point.Y == value)
                        return point;
            }

            return new PointF(0, 0);
        }

        public List<PointF> Points()
        {
            return points;
        }

        public int Count()
        {
            return points.Count;
        }

        /// <summary>
        /// Eldönti, hogy az objektum által reprezentált egyenes széles, vagy magas.
        /// </summary>
        /// <param name="min">Az X/Y irányú minumum érték kerül bele.</param>
        /// <param name="max">Az X/Y irányú maximum érték kerül bele.</param>
        /// <returns>Széles vonal esetén TRUE, ellenben FALSE.</returns>
        private bool isWide(out float min, out float max)
        {
            float minX = points[0].X;
            float maxX = minX;
            float minY = points[0].Y;
            float maxY = minY;

            foreach (PointF point in points)
            {  //min és max X érték keresése
                if (point.X < minX)
                    minX = point.X;
                else
                    if (point.X > maxX)
                        maxX = point.X;

                //min és max Y érték keresése
                if (point.Y < minY)
                    minY = point.Y;
                else
                    if (point.Y > maxY)
                        maxY = point.Y;
            }

            if (maxX - minX > maxY - minY)
            {
                min = minX;
                max = maxX;
                return true; //széles
            }
            else
            {
                min = minY;
                max = maxY;
                return false; //magas
            }
        }
    }
}