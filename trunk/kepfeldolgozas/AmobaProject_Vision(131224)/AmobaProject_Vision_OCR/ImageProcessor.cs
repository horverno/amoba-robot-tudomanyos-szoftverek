using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ImgPrcUtility;
using AmobaProject_Vision_Processor;


namespace AmobaProject_Vision_OCR
{
    public class ImageProcessor
    {
        //settings
        public bool equalizeHist = false;
        public bool noiseFilter = false;
        public int cannyThreshold = 50;
        public bool blur = true;
        public int adaptiveThresholdBlockSize = 4;
        public double adaptiveThresholdParameter = 1.2d;
        public bool addCanny = true;
        public bool filterContoursBySize = true;
        public bool onlyFindContours = false;
        public int minContourLength = 15;
        public int minContourArea = 10;
        public double minFormFactor = 0.5;
        //
        public List<Contour<Point>> contours;
        public Templates templates = new Templates();
        public Templates samples = new Templates();
        public List<FoundTemplateDesc> foundTemplates = new List<FoundTemplateDesc>();
        public TemplateFinder finder = new TemplateFinder();
        public Image<Gray, byte> binarizedFrame;

        public Image<Bgr, Byte> ProcessImage(Image<Bgr, byte> frame, out List<int[]> foundXO)
        {
            return ProcessImage(frame.Convert<Gray, Byte>(), out foundXO);
        }

        public Image<Bgr, Byte> ProcessImage(Image<Gray, byte> grayFrame, out List<int[]> foundXO)
        {
            Image<Bgr, Byte> rv = grayFrame.Copy().Convert<Bgr, Byte>();
            if (equalizeHist)
                grayFrame._EqualizeHist();//autocontrast
            //smoothed
            Image<Gray, byte> smoothedGrayFrame = grayFrame.PyrDown();
            smoothedGrayFrame = smoothedGrayFrame.PyrUp();
            //canny
            Image<Gray, byte> cannyFrame = null;
            if (noiseFilter)
                cannyFrame = smoothedGrayFrame.Canny(cannyThreshold, cannyThreshold);
            //smoothing
            if (blur)
                grayFrame = smoothedGrayFrame;
            //binarize
            CvInvoke.cvAdaptiveThreshold(grayFrame, grayFrame, 255, Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, adaptiveThresholdBlockSize + adaptiveThresholdBlockSize % 2 + 1, adaptiveThresholdParameter);
            //
            grayFrame._Not();
            //
            if (addCanny)
                if (cannyFrame != null)
                    grayFrame._Or(cannyFrame);
            //
            this.binarizedFrame = grayFrame;

            //dilate canny contours for filtering
            if (cannyFrame != null)
                cannyFrame = cannyFrame.Dilate(3);

            //find contours
            var sourceContours = grayFrame.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST);
            //filter contours
            contours = FilterContours(sourceContours, cannyFrame, grayFrame.Width, grayFrame.Height);
            //find templates
            lock (foundTemplates)
                foundTemplates.Clear();
            samples.Clear();

            lock (templates)
                Parallel.ForEach<Contour<Point>>(contours, (contour) =>
                {
                    var arr = contour.ToArray();
                    Template sample = new Template(arr, contour.Area, samples.templateSize);
                    lock (samples)
                        samples.Add(sample);

                    if (!onlyFindContours)
                    {
                        FoundTemplateDesc desc = finder.FindTemplate(templates, sample);

                        if (desc != null)
                            lock (foundTemplates)
                                foundTemplates.Add(desc);
                    }
                }
                );

            FilterByIntersection(ref foundTemplates);
            foundXO = new List<int[]>();

            foreach (FoundTemplateDesc found in foundTemplates)
            {
                if (found.template.name.EndsWith(".png") || found.template.name.EndsWith(".jpg"))
                {
                    continue;
                }

                Rectangle foundRect = found.sample.contour.SourceBoundingRect;

                Point p1 = new Point((foundRect.Left + foundRect.Right) / 2, foundRect.Top);
                string text = found.template.name;
                MCvFont f = new MCvFont(FONT.CV_FONT_HERSHEY_COMPLEX, 1.0, 1.0);
                //rv.Draw((found.angle*180/Math.PI).ToString(), ref f, foundRect.Center(), new Bgr(0, 0, 0));
                
                if (text == "X" && foundRect.Area() < 4000 && foundRect.Area() > 1000)
                {
                    rv.Draw(foundRect, new Bgr(0, 255, 0), 4);
                    foundXO.Add(new int[5] {foundRect.Center().X, foundRect.Center().Y, 1, foundRect.Width, foundRect.Height});
                }
                else if (text == "O" && foundRect.Area() < 4000 && foundRect.Area() > 1000)
                {
                    rv.Draw(foundRect, new Bgr(0, 0, 255), 4);
                    foundXO.Add(new int[5] { foundRect.Center().X, foundRect.Center().Y, 0, foundRect.Width, foundRect.Height });
                }
                //rv.Draw(new CircleF(new PointF(foundRect.Center().X, foundRect.Center().Y), 10), new Bgr(0, 255, 0), 3);
                //e.Graphics.DrawRectangle(borderPen, foundRect);
                //e.Graphics.DrawString(text, font, bgBrush, new PointF(p1.X + 1 - font.Height / 3, p1.Y + 1 - font.Height));
                //e.Graphics.DrawString(text, font, foreBrush, new PointF(p1.X - font.Height / 3, p1.Y - font.Height));
            }
            return rv;
        }

        private static void FilterByIntersection(ref List<FoundTemplateDesc> templates)
        {
            //sort by area
            templates.Sort(new Comparison<FoundTemplateDesc>((t1, t2) => -t1.sample.contour.SourceBoundingRect.Area().CompareTo(t2.sample.contour.SourceBoundingRect.Area())));
            //exclude templates inside other templates
            HashSet<int> toDel = new HashSet<int>();
            for (int i = 0; i < templates.Count; i++)
            {
                if (toDel.Contains(i))
                    continue;
                Rectangle bigRect = templates[i].sample.contour.SourceBoundingRect;
                int bigArea = templates[i].sample.contour.SourceBoundingRect.Area();
                bigRect.Inflate(4, 4);
                for (int j = i + 1; j < templates.Count; j++)
                {
                    if (bigRect.Contains(templates[j].sample.contour.SourceBoundingRect))
                    {
                        double a = templates[j].sample.contour.SourceBoundingRect.Area();
                        if (a / bigArea > 0.9d)
                        {
                            //choose template by rate
                            if (templates[i].rate > templates[j].rate)
                                toDel.Add(j);
                            else
                                toDel.Add(i);
                        }
                        else//delete tempate
                            toDel.Add(j);
                    }
                }
            }
            List<FoundTemplateDesc> newTemplates = new List<FoundTemplateDesc>();
            for (int i = 0; i < templates.Count; i++)
                if (!toDel.Contains(i))
                    newTemplates.Add(templates[i]);
            templates = newTemplates;
        }

        private List<Contour<Point>> FilterContours(Contour<Point> contours, Image<Gray, byte> cannyFrame, int frameWidth, int frameHeight)
        {
            int maxArea = frameWidth * frameHeight / 5;
            var c = contours;
            List<Contour<Point>> result = new List<Contour<Point>>();
            while (c != null)
            {
                if (filterContoursBySize)
                    if (c.Total < minContourLength ||
                        c.Area < minContourArea || c.Area > maxArea ||
                        c.Area / c.Total <= minFormFactor)
                        goto next;

                if (noiseFilter)
                {
                    Point p1 = c[0];
                    Point p2 = c[(c.Total / 2) % c.Total];
                    if (cannyFrame[p1].Intensity <= double.Epsilon && cannyFrame[p2].Intensity <= double.Epsilon)
                        goto next;
                }
                result.Add(c);

            next:
                c = c.HNext;
            }

            return result;
        }

        private bool isHasonlo(int n1, int n2, int t)
        {
            //n1 n2+/-t-n belül van-e
            if (n1 < n2 + t && n1 > n2 - t)
                return true;
            else
                return false;
        }
    }
}