using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using ImgPrcUtility;

namespace AmobaProject_Vision_Processor
{
    public class CrossCageFinder
    {
        private List<DetectedField> detectedFields;
        private int nccta; //necessaryCalibrationCountToAccept
        private int nfcta; //necessaryCalibrationCountToAccept

        private float avgSize;
        private float avgAngle;
        private PointF avgCenter;

        public CrossCageFinder()
        {
            detectedFields = new List<DetectedField>();
            nccta = 10; 
            nfcta = 5;
        }

        public DetectedField GeneralField(Image<Bgr, Byte> img)
        {
            //Detect all possible boxes by opencv
            List<MCvBox2D> allBoxes = RealBoxes(Boxes(img));

            //Opencv can recognise redundant boxes at the same place so I have to remove some of them
            List<MCvBox2D> uniqueBoxes = UniqueBoxes(allBoxes);

            //There can be more boxes out of the field but we need only the boxes on field
            //List<MCvBox2D> relevantBoxes = Coherent(RelevantBoxes(uniqueBoxes));
            List<MCvBox2D> relevantBoxes = RelevantBoxes(uniqueBoxes);

            //Some avarage properties
            avgCenter = AvgCenter(relevantBoxes);
            avgAngle = AvgAngle(relevantBoxes);
            avgSize = AvgSize(relevantBoxes);

            //csinálok egy pályakeretet
            MCvBox2D frameBox = new MCvBox2D();
            //ha megfelelő szögben áll
            if (Math.Abs(avgAngle) < 3 || Math.Abs(avgAngle) > 87)
                frameBox = StraightFrame(avgSize, relevantBoxes);
            else
                frameBox = CrossFrame(avgSize, relevantBoxes);

            return new DetectedField(frameBox, avgSize);
        }

        #region FindFrame
        private List<MCvBox2D> Boxes(Image<Bgr, Byte> img)
        {   //It's a sample in opencv tutorials. It returns with detected boxes.
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

        private List<MCvBox2D> RealBoxes(List<MCvBox2D> boxList)
        {   //OpenCV can find boxes, which size is very small or zero.
            List<MCvBox2D> realBoxes = new List<MCvBox2D>();

            foreach (MCvBox2D box in boxList)
                if (box.size.Width * box.size.Height > 400)
                    realBoxes.Add(box);

            return realBoxes;
        }

        private List<MCvBox2D> UniqueBoxes(List<MCvBox2D> boxList)
        {   //It makes redundant boxes if there is more than one
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

        private List<MCvBox2D> RelevantBoxes(List<MCvBox2D> boxList)
        {   //It returns only with the relevant boxes. The main frame will consist these
            List<MCvBox2D> relevantBoxes = new List<MCvBox2D>();

            //Categorize by size
            Dictionary<Size, List<MCvBox2D>> categories = new Dictionary<Size, List<MCvBox2D>>();
            foreach (MCvBox2D box in boxList)
            {
                Size result = new Size(0, 0);
                Size current = new Size((int)box.size.Width, (int)box.size.Height);
                if (isRect(current, 15))
                {   //If it's a square
                    foreach (Size key in categories.Keys)
                    {
                        if (isTheSameSize(key, current, 7))
                        {
                            result = key;
                        }
                    }
                }

                //If result has changed
                if (result.Width > 0 && result.Height > 0)
                    categories[result].Add(box);
                else
                {   //Make a List with Key identification
                    if (!categories.Keys.Contains(current))
                    {
                        categories.Add(current, new List<MCvBox2D>());
                        categories[current].Add(box);
                    }
                    else
                        categories[current].Add(box);
                }
            }

            //Looking for the list which has the most value
            int maxCount = 0;
            foreach (Size key in categories.Keys)
            {
                if (categories[key].Count > maxCount)
                {
                    maxCount = categories[key].Count;
                    relevantBoxes = categories[key];
                }
            }

            return relevantBoxes;
        }

        private List<MCvBox2D> Coherent(List<MCvBox2D> boxList)
        {   //returns with the related boxes
            List<MCvBox2D> coherent = new List<MCvBox2D>();
            float diff = (float)(avgSize * 1.3);
            
            //Choose a start item
            foreach (MCvBox2D first in boxList)
            {
                coherent.Clear();
                coherent.Add(first);

                bool match = true;
                while (match)
                {
                    match = false;
                    foreach (MCvBox2D a in coherent)
                    {
                        foreach (MCvBox2D b in boxList)
                        {
                            if (!coherent.Contains(b))
                                if (isNearby(a, b, diff))
                                {
                                    coherent.Add(b);
                                    match = true;
                                }
                        }

                        if (match)
                            break;
                    }

                }

                if (coherent.Count >= (boxList.Count * 0.5))
                    break;

            }
            return coherent;
        }

        private bool isTheSamePoint(Point innerPoint, Point outerPoint, int threshold)
        {   //It returns true if p1 point is around p2 point maximum t far away.
            if ((innerPoint.X < outerPoint.X + threshold &&
                 innerPoint.X > outerPoint.X - threshold) &&
                 innerPoint.Y < outerPoint.Y + threshold &&
                 innerPoint.Y > outerPoint.Y - threshold)
                return true;
            else
                return false;
        }

        private bool isTheSameSize(Size size1, Size size2, int threshold)
        {
            if (Math.Abs(size1.Width - size2.Width) < threshold && Math.Abs(size1.Height - size2.Height) < threshold)
                return true;
            else
                return false;
        }

        private bool isNearby(MCvBox2D b1, MCvBox2D b2, float t)
        {   //Are these neighboring?
            float x = Math.Abs(b1.center.X - b2.center.X);
            float y = Math.Abs(b1.center.Y - b2.center.Y);
            if (Math.Sqrt(x * x + y * y) <= t)
                return true;
            else
                return false;
        }

        private bool isRect(Size s, int t)
        {   //Is it a Rect...? :)
            if (Math.Abs(s.Width - s.Height) < t)
                return true;
            else
                return false;
        }
        #endregion FindFrame
        
        #region MakeField
        private MCvBox2D StraightFrame(float size, List<MCvBox2D> boxList)
        {   //If the table is straight than I can create a frame easily
            float minX = boxList[0].center.X;
            float maxX = minX;
            float minY = boxList[0].center.Y;
            float maxY = minY;

            foreach (MCvBox2D box in boxList)
            {
                float bcx = box.center.X;
                float bcy = box.center.Y;

                if (bcx < minX)
                    minX = bcx;
                else
                    if (bcx > maxX)
                        maxX = bcx;

                if (bcy < minY)
                    minY = bcy;
                else
                    if (bcy > maxY)
                        maxY = bcy;
            }

            float width = (float)((maxY - minY + size));
            float height = (float)((maxX - minX + size));

            //Sometimes there is a problem if avgAngle is close to zero or 90 degree
            if (Math.Abs(avgAngle) > 88)
                return new MCvBox2D(avgCenter, new SizeF(width, height), avgAngle);
            else
                return new MCvBox2D(avgCenter, new SizeF(height, width), avgAngle);

        }

        private MCvBox2D CrossFrame(float size, List<MCvBox2D> boxList)
        {   //Fabricate the cross frame with hough transform
            List<MyLine> lineList = LinesWithHough(boxList);

            List<MyLine> horizontalLine = SuitableLines(avgAngle, lineList, "h");
            List<MyLine> verticalLine = SuitableLines(avgAngle, lineList, "v");
            float bigBoxWidth = LongestLine(horizontalLine) + size;
            float bigBoxHeight = LongestLine(verticalLine) + size;

            if (Math.Abs(avgAngle) < 45) //opencv specificity, it handles the angle of boxes oddly 
                return new MCvBox2D(AvgCenter(boxList), new SizeF(bigBoxWidth, bigBoxHeight), avgAngle);
            else
                return new MCvBox2D(AvgCenter(boxList), new SizeF(bigBoxHeight, bigBoxWidth), avgAngle);
        }

        private float LongestLine(List<MyLine> lines)
        {   //a kapott vonallistából visszaadom a leghosszabbat
            float max = 0;
            foreach (MyLine l in lines)
            {
                if (l.Length() > max)
                    max = l.Length();
            }
            return max;
        }

        private List<MyLine> LinesWithHough(List<MCvBox2D> boxList)
        {   //Recognise lines with hough
            Dictionary<PointF, PointsOfLine> allSinusoid = new Dictionary<PointF, PointsOfLine>();
            List<MyLine> lines = new List<MyLine>();

            //Draw a sinusoidal with value form box.center
            foreach (MCvBox2D box in boxList)
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

            //We need the key where is the most objects
            foreach (PointF key in allSinusoid.Keys)
            {
                if (allSinusoid[key].Count() > 1)
                {
                    PointF a = allSinusoid[key].getA();
                    PointF b = allSinusoid[key].getB();

                    //It was the same two or three times so...
                    if (a.X != b.X || a.Y != b.Y)
                    {
                        //Because of rounding we have to remove the redundant lines
                        bool duplicated = false;
                        foreach (MyLine line in lines)
                        {
                            //hasonlítandó elem
                            PointF c = line.pointA;
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

        private List<MyLine> SuitableLines(float boxDegree, List<MyLine> lineList, string mod)
        {   //Returns with lines, which have suitable degree
            //... so the boxDegree and boxDegree+90
            List<MyLine> result = new List<MyLine>();

            foreach (MyLine line in lineList)
            {
                float bd; //boxDegree
                float ld; //lineDegree
                if (mod.Equals("h"))
                {
                    bd = (boxDegree + 90) * -1;
                    ld = line.Degree("h");
                }
                else
                {
                    bd = boxDegree;
                    ld = line.Degree("v");
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
        {   //returns with avg angle of boxes
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

        private PointF AvgCenter(List<MCvBox2D> boxes)
        {   //returns with avg center of boxes
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
}