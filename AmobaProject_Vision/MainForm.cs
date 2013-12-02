using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

using AmobaProject_Vision_OCR;


namespace AmobaProject_Vision
{
    public partial class MainForm : Form
    {

        private Capture capture = null;
        private bool captureInProgress;
        private bool detectGrid;
        private bool detectXO;
        private ImageProcessor processor;
        private GameboardDrawer board;
        private int fc;
        private bool rotate;
        private Calibrator calib;

        public MainForm()
        {
            
            InitializeComponent();
            try
            {
                calib = new Calibrator();
                captureInProgress = true;
                detectGrid = true;
                detectXO = true;
                rotate = false;
                processor = new ImageProcessor();
                processor.equalizeHist = false;
                processor.finder.maxRotateAngle = Math.PI;
                processor.minContourArea = 10;
                processor.minContourLength = 15;
                processor.finder.maxACFDescriptorDeviation = 4;
                processor.finder.minACF = 0.96;
                processor.finder.minICF = 0.85;
                processor.blur = true;
                processor.noiseFilter = false;
                processor.cannyThreshold = 50;
                processor.adaptiveThresholdBlockSize = 4;
                processor.adaptiveThresholdParameter = 1.5;
                processor.templates.Clear();
                TemplateGenerator.GenerateChars(processor, new char[] { 'X', 'O' }, new System.Drawing.Font(new FontFamily("Consolas"), 100, FontStyle.Regular));

                capture = new Capture();
                capture.ImageGrabbed += ProcessFrame;
                capture.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            List<int[]> foundXO = new List<int[]>(); //megtalált bábuk adatai
            board = new GameboardDrawer(1, 1, 400, 400, 2);
            Image<Bgr, Byte> frame = capture.RetrieveBgrFrame(); //Aktuális kamerakép
            if (rotate)
            {
                frame = frame.Rotate(180, new Bgr(0, 0, 0));
            }
            imbOriginal.Image = frame; //eredeti kép betöltése imagebox-ba

            Image<Bgr, Byte> frameMod = frame; //ez lesz a módosított kép (detektálás)

            if (detectXO) //Karakterfelismerés, ha be van pipálva
            {
                frameMod = processor.ProcessImage(frame, out foundXO); //Visszaadja a képet, amin bekeretezte a karaktereket, illetve megkapjuk a bábuk adatait
            }

            if (detectGrid) //játékmező kirajzolás, ha be  van pipálva
            {
                List<MCvBox2D> boxlist = new List<MCvBox2D>(); //ebben tároljuk a felismert négyszögeket
                GetRectangles(frame, out boxlist); // megkeresi és beteszi a listába a négyszögeket
                if (boxlist.Count > 0) //ha talált négyszögeket, akkor megalkotja belőle a rácsot
                {
                    int maxTop;
                    int maxBottom;
                    int maxRight;
                    int maxLeft;

                    GetGridEdge(boxlist, out maxTop, out maxBottom, out maxRight, out maxLeft); //megkeresi a felismert négyszöget közül a legszélők koordinátáit

                    int x;
                    int y;

                    GetGridCellsNum(boxlist, out x, out y); //a felismert négyszögek alapján meghatározza, hogy hányszor hányas lehet a játékmező

                    calib.AddValue(x, y);


                    board = new GameboardDrawer(x, y, 400, 400, 2);

                        int width = maxRight - maxLeft;
                        int cellWidth = width / x;
                        int height = maxBottom - maxTop;
                        int cellHeight = height / y;
                    if (cellWidth > 0 && cellHeight > 0)
                    {
                        for (int i = 0; i < foundXO.Count; i++)
                        {
                            if (foundXO[i][2] == 1)
                            {
                                if ((foundXO[i][0] - maxLeft) >= 0 && (foundXO[i][0] - maxLeft) < width && (foundXO[i][1] - maxTop) >= 0 && (foundXO[i][1] - maxTop) < height)
                                {
                                    board.setX((foundXO[i][0] - maxLeft) / cellWidth, (foundXO[i][1] - maxTop) / cellHeight);
                                }
                            }
                            else if (foundXO[i][2] == 0)
                            {
                                if ((foundXO[i][0] - maxLeft) >= 0 && (foundXO[i][0] - maxLeft) < width && (foundXO[i][1] - maxTop) >= 0 && (foundXO[i][1] - maxTop) < height)
                                {
                                    board.setO((foundXO[i][0] - maxLeft) / cellWidth, (foundXO[i][1] - maxTop) / cellHeight);
                                }
                            }
                        }
                    }
                    frameMod = DrawGrid(frameMod, new Point(maxLeft, maxTop), new Point(maxRight, maxTop), new Point(maxLeft, maxBottom), new Point(maxRight, maxBottom), x, y); //az előző adatok alapján rárajzolja a képre a játékmezőt
                }
            }

            imbModified.Image = frameMod;
            if (calib.Calibrated())
            {
                imageBox1.Image = board.getGameboard();
            }
            else
            {
                imageBox1.Image = new Image<Bgr, byte>(640, 480); //KEP MERETE NEM LEHET 1*1. MIERT????
            }

        }
        
        //Kamera indítása/leállítása
        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (capture != null)
            {
                if (captureInProgress)
                {
                    capture.Pause();
                    btnCapture.Text = "Start";

                }
                else
                {
                    capture.Start();
                    btnCapture.Text = "Stop";
                }

                captureInProgress = !captureInProgress;
            }
        }

        //ha úgy zárjuk be a programot, hogy előtte nem állítottk le a kamerát, akkor hibával állna le a program. Ezt kerüli el ez a pár sor
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) 
        {
            if (capture != null)
            {
                capture.Stop();
                capture.Dispose();
            }
        }

        private void chbGrid_CheckedChanged(object sender, EventArgs e)
        {
            detectGrid = !detectGrid;
        }

        private void chbXO_CheckedChanged(object sender, EventArgs e)
        {
            detectXO = !detectXO;
        }

        //Rácsot rajzol a megadott képre, a megadott sarokpontok és cellaszámok alapján
        private Image<Bgr, Byte> DrawGrid(Image<Bgr, Byte> img, Point lu, Point ru, Point ld, Point rd, int cellNumX, int cellNumY)
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
            return result;
        }

        //megkeresi a négyszögeket a képen. A felismert négyszögeket méretük alapján megpróbálja kategóriákba sorolni és csak azokat tekinti négyszögnek, amelyek a legtöbb elemszámú kategóriába esnek. Ezzel elkerülve, hogy pl a rajzlapot is felismerje négyszögként
        private Image<Bgr, Byte> GetRectangles(Image<Bgr, Byte> img, out List<MCvBox2D> boxlist)
        {
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();

            Image<Gray, Byte> cannyEdges =  img.Canny(100.0, 60.0);


            List<MCvBox2D> boxList = new List<MCvBox2D>();
            Contour<Point> contours;
            using (MemStorage storage = new MemStorage())
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

            Image<Bgr, Byte> triangleRectangleImage = img.Copy();
            bool tmp = false;
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

                    triangleRectangleImage.Draw(boxList[i], new Bgr(0, 255, 0), 2);

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
            return triangleRectangleImage; //cannyEdges.Convert<Bgr, Byte>();
        }

        //a már felismert négyszögek alapján kitalálja, hogy hányszor hányas lehet az eredeti pálya
        private void GetGridCellsNum(List<MCvBox2D> boxlist, out int x, out int y)
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

        //a már felismert négyszögek alapján megkeresi azt a négy négyszöget ami a pálya legszélein vannak (fent, lent, balra, jobbra) és ezek koordináit adja vissza. Ezek a koordináták fogják meghatározni a berajzolt rács sarokpontjait. És egyben ezért nem lesz jó a kép, ha nem egyenesen áll a pálya
        private void GetGridEdge(List<MCvBox2D> boxlist, out int maxTop, out int maxBottom, out int maxRight, out int maxLeft)
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

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(fc.ToString());
            /*GameboardDrawer gbd1 = new GameboardDrawer(4, 4, 200, 200, 2);
            gbd1.setX(2, 3);
            gbd1.setO(0, 0);
            imbModified.Image = gbd1.getGameboard();*/
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            rotate = !rotate;
        }

    }
}
