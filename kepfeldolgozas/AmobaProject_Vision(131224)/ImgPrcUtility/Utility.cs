using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using System.Drawing;

namespace ImgPrcUtility
{
    public class DetectedField
    {
        public MCvBox2D frameBox { get; set; }
        public int rowCount { get; set; }
        public int colCount { get; set; }
        public float avgBoxSize { get; set; }

        public DetectedField(MCvBox2D frameBox, float avgFieldSize)
        {   //constructor
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

        public bool isTheSame(Object obj)
        {   //It compares two DetectedField objects. If those are the same it returns true value.
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

        public String toString()
        {
            return "A négyzet: " + colCount + " széles, és " + rowCount + " magas.";
        }
    }

    /// <summary>
    /// ImgPrcPuppetFinder use this class to save detected puppets properties.
    /// </summary>
    public class DetectedPuppet
    {
        public Rectangle puppetRect { get; set; }
        public String puppetValue { get; set; }
        public Point center { get; set; }

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
    /// It's necessary to recognise the cross field.
    /// </summary>
    public class MyLine
    {
        public PointF pointA { get; set; }
        public PointF pointB { get; set; }
        private float length;

        public MyLine(PointF pointA, PointF pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
            length = Length();
        }

        public float Degree(string mod)
        {   //It returns with the degree of line object.
            float width = Math.Abs(pointB.X - pointA.X);
            float height = Math.Abs(pointB.Y - pointA.Y);
            if (mod.Equals("h"))
            {
                width = Math.Abs(pointB.Y - pointA.Y);
                height = Math.Abs(pointB.X - pointA.X);
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
        {   //Length of line...
            return (float)Math.Sqrt(Math.Pow(pointA.X - pointB.X, 2) +
                          Math.Pow(pointA.Y - pointB.Y, 2));
        }

        private double toDegree(double angle)
        {
            return 180.0 * angle / Math.PI;
        }
    }

    /// <summary>
    /// This class handles the points of a sinusoidal.
    /// It's necessary to recognise the cross field with Hough transform.
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

        public PointF getA()
        {  //visszaadja az egyenes egyik végpontját (bal alsó/felső)
            if (isWide(out min, out max) == true)
                return getThis('x', min);
            else
                return getThis('y', min);

        }

        public PointF getB()
        {   //visszaadja az egyenes másik végpontját (jobb alsó/felső)
            if (isWide(out min, out max) == true)
                return getThis('x', max);
            else
                return getThis('y', max);

        }

        private PointF getThis(char xory, float value)
        {
            foreach (PointF point in points)
            {
                if (xory.Equals('x'))
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

        private bool isWide(out float min, out float max)
        {   /* If the line, which represented by point is wide then it returns true
             * else false.
             */
            float minX = points[0].X;
            float maxX = minX;
            float minY = points[0].Y;
            float maxY = minY;

            foreach (PointF point in points)
            {
                //min és max X érték keresése
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
