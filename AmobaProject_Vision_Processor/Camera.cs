using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;

using AmobaProject_Vision_OCR;
using InterfaceModule;

using System.Windows.Forms;


namespace AmobaProject_Vision_Processor
{
    public class Camera : IImageProcessing
    {

        //Ezekre az eseményekre iratkozik fel a játéklogika csapat.
        public event EventHandler<GameStatusChangedEventArgs> GameStatusChanged; 
        public event EventHandler<TableStateChangedEventArgs> TableStateChanged;
        public event EventHandler<NextPieceChangedEventArgs> NextPieceChanged;
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        public Image<Bgr, byte> OriginalImage; //Az eredeti kép.
        public Image<Bgr, byte> DetectedImage; //Eredeti kép a felismert alakzatokkal.
      
        private Capture capture = null; //Kamera.
        private CharacterDetector ocr; //Karakter felismerő.
        private GameboardDrawer prevBoard; //Előző tábla állapota.
        private GameboardDrawer board; //Aktuális tábla állapota.
        private Calibrator calib; //Kalibrátor.
        private Boolean nextPiece; //Következő bábu állapota.
        private Boolean prevPiece; //Előző bábu állapota.

        private bool calOk; //Kalibrálás sikeressége.
        private bool newGame; //Új játék kezdése.

        private int maxTop; //Pálya sarokpont koordináta.
        private int maxBottom; //Pálya sarokpont koordináta.
        private int maxRight; //Pálya sarokpont koordináta.
        private int maxLeft; //Pálya sarokpont koordináta.
        private int row; //Pálya sorszáma.
        private int column; //Pálya oszlopszáma.

        private GameStatus status; //Kamera és pálya elérhetősége.

        private Rectangle[,] boxBounds; //Mezőhatárok.

        PictureBox ibOriginal; //Eredeti kamerakép erre kerül.
        PictureBox ibDetected; //Módosított kamerakép erre kerül.
        PictureBox ibDraw; //Játéknézeti kép erre kerül.

        private int tableCalibX; //Kalibrációs pont.
        private int tableCalibY; //Kalibrációs pont.
        private int tableCalibSens; //Kalibráció érzékenység.

        private int nextPieceCalibX; //Következő bábu pozíciója.
        private int nextPieceCalibY; //Következő bábu pozíciója.
        private int nextPieceCalibSens; //Következő bábu kalibrációs érzékenysége

        private Piece nextPieceType;

        public void Init(PictureBox ibOriginal, PictureBox ibDetected, PictureBox ibDraw, int tableCalibX, int tableCalibY, int tableCalibSens, int nextPieceCalibX, int nextPieceCalibY, int nextPieceCalibSens, string fontType) //Inicializálás
        {
            try
            {
                calOk = false;
                newGame = true;
                this.ibOriginal = ibOriginal;
                this.ibDetected = ibDetected;
                this.ibDraw = ibDraw;

                this.tableCalibX = tableCalibX;
                this.tableCalibY = tableCalibY;
                this.tableCalibSens = tableCalibSens;
                this.nextPieceCalibX = nextPieceCalibX;
                this.nextPieceCalibY = nextPieceCalibY;
                this.nextPieceCalibSens = nextPieceCalibSens;

                /*tableCalibX = 17;
                tableCalibY = 452;
                tableCalibSens = 40;
                nextPieceCalibX = 574;
                nextPieceCalibY = 359;
                nextPieceCalibSens = 10;*/

                calib = new Calibrator(tableCalibSens, tableCalibX, tableCalibY);
                ocr = new CharacterDetector();
                ocr.equalizeHist = false;
                ocr.finder.maxRotateAngle = Math.PI;
                ocr.minContourArea = 10;
                ocr.minContourLength = 15;
                ocr.finder.maxACFDescriptorDeviation = 4;
                ocr.finder.minACF = 0.96;
                ocr.finder.minICF = 0.85;
                ocr.blur = true;
                ocr.noiseFilter = false;
                ocr.cannyThreshold = 50;
                ocr.adaptiveThresholdBlockSize = 4;
                ocr.adaptiveThresholdParameter = 1.5;
                ocr.templates.Clear();
                TemplateGenerator.GenerateChars(ocr, new char[] { 'X', 'O' }, new System.Drawing.Font(new FontFamily(fontType), 100, FontStyle.Regular));

                status = GameStatus.Online;
                capture = new Capture();
                capture.ImageGrabbed += ProcessFrame;
                OnGameStatusChanged(GameStatus.Online);
                prevPiece = false;
                nextPiece = false;
            }
            catch 
            {
                OnGameStatusChanged(GameStatus.Offline); //Ha a kamera nem található.
            }
        }

        public void NewGame() //Új játék indítása.
        {
            calib = new Calibrator(tableCalibSens, tableCalibX, tableCalibY);
            newGame = true;
            calOk = false;
        }

        private void ProcessFrame(object sender, EventArgs arg) //Képenkénti kamera műveletek.
        {
            OriginalImage = capture.RetrieveBgrFrame(); //Kamera kép lekérdezése.
            DetectedImage = OriginalImage.Copy();
            
            if (!calOk) //Kalibráció
            {
                if (newGame) //Ha új játék kezdődik.
                {
                    status = GameStatus.SearchBoard;
                    OnGameStatusChanged(status); //Új pálya keresése.
                    newGame = false;
                }

                List<MCvBox2D> boxlist = new List<MCvBox2D>(); //Felismert négyszögek tárolása.

                GetRectangles(OriginalImage, out boxlist); //Felismert négyszögek lekérdezése.

                if (boxlist.Count > 0) //Ha találtunk téglalapot.
                {
                    GetGridEdge(boxlist, out maxTop, out maxBottom, out maxRight, out maxLeft); //Megkeresi a felismert négyszöget közül a legszélsők koordinátáit.
                    GetGridCellsNum(boxlist, out row, out column); //A felismert négyszögek alapján meghatározza, hogy hányszor hányas lehet a játékmező.
                    calib.AddValue(row, column, maxLeft, maxBottom); //Felismert pálya adatait beadja a kalibrátornak.

                    board = new GameboardDrawer(row, column, 400, 400, 5, 300); //Létrehozza a táblát.

                    if (calib.Calibrated() && row == column && (row == 4 || row == 3)) //Ha a pálya be van kalibrálva és megfelelő a sor és oszlop szám.
                    {
                        calOk = true;
                        if (row == 3)
                        {
                            status = GameStatus.BoardDetected_3x3;
                        }
                        else
                        {
                            status = GameStatus.BoardDetected_4x4;
                        }
                        OnGameStatusChanged(status); //Pálya megtalálva esemény küldése.
                    }
                    else
                    {
                        DetectedImage = DrawCalib(DetectedImage); //Kirajzolunk egy derékszöget, és a pillanatnyi helyzetét a pályának.
                    }
                }

                Point corner = calib.getCorner(); //Segédvonal koordinátái.

                //kirajzoljuk a pálya kalibrálását segítő vonalakat
                DetectedImage.Draw(new LineSegment2D(new Point(corner.X - 20, corner.Y),
                                                     new Point(corner.X + 100, corner.Y)),
                                   new Bgr(0, 127, 0), 4);

                DetectedImage.Draw(new LineSegment2D(new Point(corner.X, corner.Y + 20),
                                                     new Point(corner.X, corner.Y - 100)),
                                   new Bgr(0, 127, 0), 4);
            }
            else //Be van kalibrálva.
            {
                List<int[]> foundXO = new List<int[]>(); //Megtalált bábuk tárolása.
                DetectedImage = ocr.ProcessImage(OriginalImage, out foundXO); //Megkeresi a bábukat (karaktereket) és bekeretezi a képen.
                prevBoard = board.Copy(); //Pálya előző állapotának elmentése.

                int width = maxRight - maxLeft;
                int cellWidth = width / row;
                int height = maxBottom - maxTop;
                int cellHeight = height / column;

                List<int[]> detectedPupettList = new List<int[]>();
                if (cellWidth > 0 && cellHeight > 0)
                {
                    nextPiece = false;
                    for (int i = 0; i < foundXO.Count; i++)
                    {
                        //Bábu amit felveszünk, ott van e.
                        if (isSimilar(foundXO[i][0], nextPieceCalibX, nextPieceCalibSens) && isSimilar(foundXO[i][1], nextPieceCalibY, nextPieceCalibSens))
                        {
                            nextPiece = true;
                            if (foundXO[i][2] == 1)
                            {
                                nextPieceType = Piece.X;
                            }
                            else
                            {
                                nextPieceType = Piece.O;
                            }
                        }

                        //Megtalált bábuk objektummá alakítása.
                        int xoCenterX = foundXO[i][0];
                        int xoCenterY = foundXO[i][1];
                        int xoWidth = foundXO[i][3];
                        int xoHeight = foundXO[i][4];
                        int xoLeft = xoCenterX - ((int)(xoWidth / 2));
                        int xoTop = xoCenterY - ((int)(xoHeight / 2));

                        if (foundXO[i][2] == 1)
                            detectedPupettList.Add(new int[5] {xoLeft, xoTop, xoWidth, xoHeight, 1});
                        else if (foundXO[i][2] == 0)
                            detectedPupettList.Add(new int[5] {xoLeft, xoTop, xoWidth, xoHeight, 0});
                    }

                    //Meg kell találni a pályát megint.
                    int dfWidth = maxRight - maxLeft;
                    int dfHeight = maxBottom - maxTop;
                    DetectedField detectedField = new DetectedField(new MCvBox2D(new PointF(maxLeft + (dfWidth / 2), maxTop + (dfHeight / 2)),
                                                                                 new SizeF(dfWidth, dfHeight), 0), 
                                                                   (maxRight-maxLeft)/column );
                    if(detectedField.colCount>0 && detectedField.rowCount>0)
                        setBoard(detectedField, detectedPupettList);

                }

                DetectedImage = DrawGrid(DetectedImage, new Point(maxLeft, maxTop), new Point(maxRight, maxTop), new Point(maxLeft, maxBottom), new Point(maxRight, maxBottom), row, column); //Az előző adatok alapján rárajzolja a képre a játékmezőt.
                
                if (!GameboardDrawer.Compare(prevBoard, board)) //Összehasonlítja az előző és az aktuális pályát.
                {
                    OnTableStateChanged(board.GetTable()); //Bábu kerül fel vagy le.
                    try
                    {
                        ibDraw.Image = board.getGameboard().Bitmap;//.Rotate(180, new Bgr(0, 0, 0));   
                    }
                    catch
                    {
                        Console.WriteLine("PictureBox error!!!!!!!!!!!!!!");
                    }
                }

                if (nextPiece != prevPiece) //Csak változás esetén legyen esemény.
                {
                    if(nextPiece)
                        OnNextPieceChanged(nextPieceType);
                    else
                        OnNextPieceChanged(Piece._NextPieceMissing);
                    prevPiece = nextPiece;
                }
            }

            //Képek visszaadása.
            try
            {
                ibOriginal.Image = OriginalImage.Bitmap;//.Rotate(180, new Bgr(0, 0, 0));
                ibDetected.Image = DetectedImage.Bitmap;//.Rotate(180, new Bgr(0, 0, 0));
            }
            catch
            {
                Console.WriteLine("PictureBox error!!!!!!!!!!!!!!");
            }
        }

        private void setBoard(DetectedField detectedField, List<int[]> puppetList) //Tábla tartalmának beállítása a nem megfelelő bábú elhelyezkedések figyelésére.
        {
            int[,] puppetMatrix = new int[detectedField.rowCount, detectedField.colCount];
            setLocation(detectedField);

            //Miután megvan hogy mely mezők hol vannak, végignézem az xoListet.
            int xe = detectedField.rowCount;
            int ye = detectedField.colCount;

            //Kezdetben -1 értéket vesz fel az összes mező (ott nincs bábu).
            for (int x = 0; x < xe; x++)
                for (int y = 0; y < ye; y++)
                {
                    puppetMatrix[x, y] = -1;
                }
           
            //Mátrix adott pozíciójára a megfelelő bábú érték beállítása.
            for (int col = 0; col < xe; col++)
                for (int row = 0; row < ye; row++)
                {
                    foreach (int[] xo in puppetList)
                    //foreach (DetectedPuppet xo in puppetList)
                    {
                        byte errorBit = 0;
                        //if (isRectInRect(xo.puppetRect, boxBounds[col, row], out errorBit))
                        if (isRectInRect(new Rectangle(xo[0], xo[1], xo[2], xo[3]), boxBounds[col, row], out errorBit))
                        {
                            //if (xo.puppetValue.Equals("O"))
                            if (xo[4]==0)
                            {
                                if (puppetMatrix[col, row] >-1)
                                {
                                    puppetMatrix[col, row] = 3; //Több bábú egy mezőben.
                                    board.setPieces(col, row, Piece._MoreThan1);
                                }
                                else
                                {
                                    puppetMatrix[col, row] = 0; //O
                                    board.setPieces(col, row, Piece.O);
                                }
                            }
                            else
                            {
                                
                                if (puppetMatrix[col, row] >-1)
                                {
                                    puppetMatrix[col, row] = 3; //Több bábú egy mezőben.
                                    board.setPieces(col, row, Piece._MoreThan1);
                                }
                                else
                                {
                                    puppetMatrix[col, row] = 1; //X
                                    board.setPieces(col, row, Piece.X);
                                }
                            }

                            if (errorBit == 1 && puppetMatrix[col, row] < 3)
                            {
                                puppetMatrix[col, row] = 2; //Bábú kilógás.
                                board.setPieces(col, row, Piece._OutOfField);
                            }
                        }
                    }
                }

            //Üres mező értékek felvitele.
            for (int i = 0; i < detectedField.rowCount; i++)
                for (int j = 0; j < detectedField.colCount; j++)
                    if (puppetMatrix[i,j]==-1)
                        board.setPieces(i, j, Piece._Empty);
        }

        private void setLocation(DetectedField field) //Mezőhatár beállítása kilógó bábúk ellenőrzéséhez.
        {
            int row = field.colCount;
            int col = field.rowCount;
            PointF center = field.frameBox.center;
            float size = (float)(field.avgBoxSize * 1.05);

            boxBounds = new Rectangle[col, row];
            int x0 = (int)(center.X - (col * size / 2));
            int y0 = (int)(center.Y - (row * size / 2));

            for (int x = 0; x < col; x++)
                for (int y = 0; y < row; y++)
                    boxBounds[x, y] = new Rectangle(x0 + (int)(x * size),
                                                            y0 + (int)(y * size),
                                                            (int)size, (int)size);
        }

        private bool isRectInRect(Rectangle r1, Rectangle r2, out byte eb) //Megvizsgálja hogy r1 benne van e r2-ben.
        {
            Point r1center = new Point((int)(r1.X + (r1.Width / 2)), (int)(r1.Y + (r1.Height / 2)));
            if (r1center.X > r2.X && r1center.Y > r2.Y && r1center.X < r2.Right && r1center.Y < r2.Bottom)
            {
                eb = 0;
                if (!(r1.Left > r2.Left && r1.Right < r2.Right &&
                  r1.Top > r2.Top && r1.Bottom < r2.Bottom))
                    eb = 1;
                return true;
            }
            else
            {
                eb = 0;
                return false;
            }
        }

        private bool isSimilar(int n1, int n2, int t) //n1 és n2 számokról eldönti, hogy a különbségük kisebb e mint t
        {
            if (n1 < n2 + t && n1 > n2 - t)
                return true;
            else
                return false;
        }

        public void Start() //Elindítja a kameraképet.
        {
            capture.Start();
        }

        public void Stop() //Leállítja a kameraképet.
        {
            capture.Stop();
        }

        private Image<Bgr, Byte> DrawGrid(Image<Bgr, Byte> img, Point lu, Point ru, Point ld, Point rd, int cellNumX, int cellNumY) //Rácsot rajzol a megadott képre, a megadott sarokpontok és cellaszámok alapján.
        {
            Image<Bgr, Byte> result = img.Copy();
            result.Draw(new LineSegment2D(lu, ru), new Bgr(255, 0, 0), 4);
            result.Draw(new LineSegment2D(ru, rd), new Bgr(255, 0, 0), 4);
            result.Draw(new LineSegment2D(rd, ld), new Bgr(255, 0, 0), 4);
            result.Draw(new LineSegment2D(ld, lu), new Bgr(255, 0, 0), 4);
            for (int i = 1; i < cellNumY; i++)
            {
                int leftDistX = (lu.X - ld.X);
                int leftDistY = (lu.Y - ld.Y);
                int leftXStep = leftDistX / cellNumY;
                int leftYStep = leftDistY / cellNumY;
                Point p3 = new Point(ld.X + i * leftXStep, ld.Y + i * leftYStep);


                int rightDistX = (ru.X - rd.X);
                int rightDistY = (ru.Y - rd.Y);
                int rightXStep = rightDistX / cellNumY;
                int rightYStep = rightDistY / cellNumY;
                Point p4 = new Point(rd.X + i * rightXStep, rd.Y + i * rightYStep);

                result.Draw(new LineSegment2D(p3, p4), new Bgr(255, 0, 0), 4);

            }
            for (int i = 1; i < cellNumX; i++)
            {
                int topDistX = (lu.X - ru.X);
                int topDistY = (lu.Y - ru.Y);
                int topXStep = topDistX / cellNumX;
                int topYStep = topDistY / cellNumX;
                Point p1 = new Point(ru.X + i * topXStep, ru.Y + i * topYStep);

                int bottomDistX = (ld.X - rd.X);
                int bottomDistY = (ld.Y - rd.Y);
                int bottomXStep = bottomDistX / cellNumX;
                int bottomYStep = bottomDistY / cellNumX;
                Point p2 = new Point(rd.X + i * bottomXStep, rd.Y + i * bottomYStep);

                result.Draw(new LineSegment2D(p1, p2), new Bgr(255, 0, 0), 4);
            }

            Bgr pieceCircleColor;
            if (prevPiece)
                pieceCircleColor = new Bgr(0, 127, 0);
            else
                pieceCircleColor = new Bgr(0, 0, 255);

            result.Draw(new CircleF(new PointF(nextPieceCalibX, nextPieceCalibY), 40), pieceCircleColor, 4);
            return result;
        }

        private Image<Bgr, Byte> GetRectangles(Image<Bgr, Byte> img, out List<MCvBox2D> boxlist) //Megkeresi a négyszögeket a képen. A felismert négyszögeket méretük alapján megpróbálja kategóriákba sorolni és csak azokat tekinti négyszögnek, amelyek a legtöbb elemszámú kategóriába esnek. Ezzel elkerülve, hogy pl a rajzlapot is felismerje négyszögként.
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();

            Image<Gray, Byte> cannyEdges = img.Canny(100.0, 60.0);

            List<MCvBox2D> boxList = new List<MCvBox2D>();
            Contour<Point> contours;
            using (MemStorage storage = new MemStorage())
            {
                for (
                   contours = cannyEdges.FindContours(
                      Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                      Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST,
                      storage);
                   contours != null;
                   contours = contours.HNext)
                {
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    if (currentContour.Total == 4 && currentContour.Area > 50)
                    {
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
                }
            }

            Image<Bgr, Byte> RectangleImage = img.Copy();
            int avg = 0;
            int max = 0;
            int[,] discVal = new int[20, 2];
            foreach (MCvBox2D box in boxList)
            {

                int i = box.MinAreaRect().Width / 10;
                if (i < 20 && i > 3)
                {
                    discVal[i, 0] += box.MinAreaRect().Width;
                    discVal[i, 1]++;
                    if (discVal[i, 1] > discVal[max, 1])
                    {
                        max = i;
                    }
                }

            }
            if (discVal[max, 1] != 0)
            {
                avg = discVal[max, 0] / discVal[max, 1];
            }

            for (int i = 0; i < boxList.Count; i++)
            {
                if (boxList[i].MinAreaRect().Width > avg - 10 && boxList[i].MinAreaRect().Width < avg + 10)
                {

                    RectangleImage.Draw(boxList[i], new Bgr(0, 255, 0), 2);

                    for (int j = i + 1; j < boxList.Count; j++)
                    {
                        double dist = Math.Sqrt((boxList[i].center.X - boxList[j].center.X) * (boxList[i].center.X - boxList[j].center.X) + (boxList[i].center.Y - boxList[j].center.Y) * (boxList[i].center.Y - boxList[j].center.Y));
                        if (dist < 10)
                        {
                            boxList.RemoveAt(j);
                        }
                    }

                }
                else
                {
                    boxList.RemoveAt(i);
                    i--;
                }

            }

            boxlist = boxList;
            return RectangleImage;
        }

        private void GetGridCellsNum(List<MCvBox2D> boxlist, out int x, out int y) //A már felismert négyszögek alapján kitalálja, hogy hányszor hányas lehet az eredeti pálya.
        {
            List<MCvBox2D> listX = boxlist.ToList();
            List<MCvBox2D> listY = boxlist.ToList();

            int actNum = 0;

            for (int i = 0; i < listX.Count; i++)
            {
                for (int j = 1; j < listX.Count; j++)
                {
                    if (Math.Abs(listX[i].center.X - listX[j].center.X) < 40)
                    {
                        listX.RemoveAt(j);
                        j--;
                    }
                }
                listX.RemoveAt(i);
                i--;
                actNum++;
            }

            x = actNum;

            actNum = 0;

            for (int i = 0; i < listY.Count; i++)
            {
                for (int j = 1; j < listY.Count; j++)
                {
                    if (Math.Abs(listY[i].center.Y - listY[j].center.Y) < 40)
                    {
                        listY.RemoveAt(j);
                        j--;
                    }
                }
                listY.RemoveAt(i);
                i--;
                actNum++;
            }

            y = actNum;
        }

        private void GetGridEdge(List<MCvBox2D> boxlist, out int maxTop, out int maxBottom, out int maxRight, out int maxLeft) //A már felismert négyszögek alapján megkeresi azt a négy négyszöget ami a pálya legszélein vannak (fent, lent, balra, jobbra) és ezek koordináit adja vissza. Ezek a koordináták fogják meghatározni a berajzolt rács sarokpontjait.
        {
            maxTop = Convert.ToInt16(boxlist[0].center.Y);
            maxBottom = Convert.ToInt16(boxlist[0].center.Y);
            maxLeft = Convert.ToInt16(boxlist[0].center.X);
            maxRight = Convert.ToInt16(boxlist[0].center.X);
            float size = 0;
            for (int i = 1; i < boxlist.Count; i++)
            {
                size += boxlist[i].size.Width;
                if (boxlist[i].center.Y < maxTop)
                {
                    maxTop = Convert.ToInt16(boxlist[i].center.Y);
                }
                if (boxlist[i].center.Y > maxBottom)
                {
                    maxBottom = Convert.ToInt16(boxlist[i].center.Y);
                }
                if (boxlist[i].center.X < maxLeft)
                {
                    maxLeft = Convert.ToInt16(boxlist[i].center.X);
                }
                if (boxlist[i].center.X > maxRight)
                {
                    maxRight = Convert.ToInt16(boxlist[i].center.X);
                }
            }
            size /= boxlist.Count;
            size /= 2;
            maxTop -= Convert.ToInt16(size);
            maxBottom += Convert.ToInt16(size);
            maxLeft -= Convert.ToInt16(size);
            maxRight += Convert.ToInt16(size);
        }

        public void OnTableStateChanged(Piece[,] newTable) //Táblaváltozás esemény.
        {
            if (TableStateChanged != null)
            {
                TableStateChanged(this, new TableStateChangedEventArgs(newTable));
            }
        }

        public void OnGameStatusChanged(GameStatus newStatus) //Kameraállapot és pályaméret esemény.
        {
            if (GameStatusChanged != null)
            {
                GameStatusChanged(this, new GameStatusChangedEventArgs(newStatus));
            }
        }

        public void OnNextPieceChanged(Piece newStatus) //Következő bábú állapotának eseménye.
        {
            if (NextPieceChanged != null)
            {
                NextPieceChanged(this, new NextPieceChangedEventArgs(newStatus));
            }
        }

        private Image<Bgr, Byte> DrawCalib(Image<Bgr, Byte> img) //Kalibrálás alatt kirajzolt kép.
        {
            Image<Bgr, Byte> result = img.Copy();
            CrossCageFinder fieldFinder = new CrossCageFinder();
            DetectedField detectedField = fieldFinder.GeneralField(result);
            if (detectedField != null)
            {
                result = CrossCageDrawer.DrawCage(result, 
                                                  detectedField.frameBox, 
                                                  detectedField.avgBoxSize, 
                                                  new Bgr(0, 0, 255));

            }

            return result;
        }
    }
}
