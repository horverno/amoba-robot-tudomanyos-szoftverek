using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AmobaProject_Vision_OCR
{

    //Komplex számok vektorát adja.
    [Serializable]
    public class Contour
    {
        Complex[] array; //A tárolt komplex számok.
        public Rectangle SourceBoundingRect;

        public Contour(int capacity)
        {
            array = new Complex[capacity];
        }

        protected Contour()
        {
        }

        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        public Complex this[int i]
        {
            get { return array[i]; }
            set { array[i] = value; }
        }

        public Contour(IList<Point> points, int startIndex, int count) //A megadott pontokat veszi fel komplex számként, a megadott mennyiségben, a megadott indextől.
            : this(count)
        {
            int minX = points[startIndex].X;
            int minY = points[startIndex].Y;
            int maxX = minX;
            int maxY = minY;
            int endIndex = startIndex + count;

            for (int i = startIndex; i < endIndex; i++)
            {
                var p1 = points[i];
                var p2 = i == endIndex - 1 ? points[startIndex] : points[i + 1];
                array[i] = new Complex(p2.X - p1.X, -p2.Y + p1.Y);

                if (p1.X > maxX) maxX = p1.X;
                if (p1.X < minX) minX = p1.X;
                if (p1.Y > maxY) maxY = p1.Y;
                if (p1.Y < minY) minY = p1.Y;
            }

            SourceBoundingRect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        public Contour(IList<Point> points)
            : this(points, 0, points.Count)
        {
        }

        public double Norma
        {
            get
            {
                double result = 0;
                foreach (var c in array)
                    result += c.NormaSquare;
                return Math.Sqrt(result);
            }
        }

        //Skaláris szorzatát képzi a két kontúrnak a megadott eltolással.
        public unsafe Complex Dot(Contour c, int shift)
        {
            var count = Count;
            double sumA = 0;
            double sumB = 0;
            fixed (Complex* ptr1 = &array[0])
            fixed (Complex* ptr2 = &c.array[shift])
            fixed (Complex* ptr22 = &c.array[0])
            fixed (Complex* ptr3 = &c.array[c.Count - 1])
            {
                Complex* p1 = ptr1;
                Complex* p2 = ptr2;
                for (int i = 0; i < count; i++)
                {
                    Complex x1 = *p1;
                    Complex x2 = *p2;
                    sumA += x1.a * x2.a + x1.b * x2.b;
                    sumB += x1.b * x2.a - x1.a * x2.b;

                    p1++;
                    if (p2 == ptr3)
                        p2 = ptr22;
                    else
                        p2++;
                }
            }
            return new Complex(sumA, sumB);
        }

        //Két kontúr interkorrelációját számítja ki a hasonlóság vizsgálatához.
        public Contour InterCorrelation(Contour c)
        {
            int count = Count;
            Contour result = new Contour(count);
            for (int i = 0; i < count; i++)
                result.array[i] = Dot(c, i);

            return result;
        }

        //Két kontúr autokorrelációját számítja ki a hasonlóság vizsgálatához.
        public unsafe Contour AutoCorrelation(bool normalize)
        {
            int count = Count / 2;
            Contour result = new Contour(count);
            fixed (Complex* ptr = &result.array[0])
            {
                Complex* p = ptr;
                double maxNormaSq = 0;
                for (int i = 0; i < count; i++)
                {
                    *p = Dot(this, i);
                    double normaSq = (*p).NormaSquare;
                    if (normaSq > maxNormaSq)
                        maxNormaSq = normaSq;
                    p++;
                }
                if (normalize)
                {
                    maxNormaSq = Math.Sqrt(maxNormaSq);
                    p = ptr;
                    for (int i = 0; i < count; i++)
                    {
                        *p = new Complex((*p).a / maxNormaSq, (*p).b / maxNormaSq);
                        p++;
                    }
                }
            }

            return result;
        }

        //Visszaadja azt a komplex számot, amelynek a legnagyobb az abszolút értéke.
        public Complex FindMaxNorma()
        {
            double max = 0d;
            Complex res = default(Complex);
            foreach (var c in array)
                if (c.Norma > max)
                {
                    max = c.Norma;
                    res = c;
                }
            return res;
        }

        //Normalizált skaláris szortatot ad vissza.
        public Complex NormDot(Contour c)
        {
            var count = this.Count;
            double sumA = 0;
            double sumB = 0;
            double norm1 = 0;
            double norm2 = 0;
            for (int i = 0; i < count; i++)
            {
                var x1 = this[i];
                var x2 = c[i];
                sumA += x1.a * x2.a + x1.b * x2.b;
                sumB += x1.b * x2.a - x1.a * x2.b;
                norm1 += x1.NormaSquare;
                norm2 += x2.NormaSquare;
            }

            double k = 1d / Math.Sqrt(norm1 * norm2);
            return new Complex(sumA * k, sumB * k);
        }

        //A kontúrok számának csökkentése vagy növelése a megadott paraméter alapján.
        public void Equalization(int newCount)
        {
            if (newCount > Count)
                EqualizationUp(newCount);
            else
                EqualizationDown(newCount);
        }

        //Kontúrok növelése.
        private void EqualizationUp(int newCount)
        {
            Complex currPoint = this[0];
            Complex[] newPoint = new Complex[newCount];

            for (int i = 0; i < newCount; i++)
            {
                double index = 1d * i * Count / newCount;
                int j = (int)index;
                double k = index - j;
                if (j == Count - 1)
                    newPoint[i] = this[j];
                else
                    newPoint[i] = this[j] * (1 - k) + this[j + 1] * k;
            }

            array = newPoint;
        }

        //Kontúrok csökkentése.
        private void EqualizationDown(int newCount)
        {
            Complex currPoint = this[0];
            Complex[] newPoint = new Complex[newCount];

            for (int i = 0; i < Count; i++)
                newPoint[i * newCount / Count] += this[i];

            array = newPoint;
        }

    }

    //Komplex számok tárolására szolgál.
    [Serializable]
    public struct Complex
    {
        public double a; //Valós rész.
        public double b; //Imaginárius rész.

        public Complex(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public static Complex FromExp(double r, double angle)
        {
            return new Complex(r * Math.Cos(angle), r * Math.Sin(angle));
        }

        public double Angle
        {
            get
            {
                return Math.Atan2(b, a);
            }
        }

        public override string ToString()
        {
            return a + "+i" + b;
        }

        //A Komplex szám abszolút értékét adja meg.
        public double Norma
        {
            get { return Math.Sqrt(a * a + b * b); }
        }

        //Az abszolút értékének négyzetét adja.
        public double NormaSquare
        {
            get { return a * a + b * b; }
        }

        public static Complex operator +(Complex x1, Complex x2)
        {
            return new Complex(x1.a + x2.a, x1.b + x2.b);
        }

        public static Complex operator *(double k, Complex x)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x, double k)
        {
            return new Complex(k * x.a, k * x.b);
        }

        public static Complex operator *(Complex x1, Complex x2)
        {
            return new Complex(x1.a * x2.a - x1.b * x2.b, x1.b * x2.a + x1.a * x2.b);
        }
        
        public double CosAngle() //A bezárt szög koszinusza.
        {
            return a / Math.Sqrt(a * a + b * b);
        }

        public Complex Rotate(double CosAngle, double SinAngle)
        {
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }

        public Complex Rotate(double Angle)
        {
            var CosAngle = Math.Cos(Angle);
            var SinAngle = Math.Sin(Angle);
            return new Complex(CosAngle * a - SinAngle * b, SinAngle * a + CosAngle * b);
        }
    }

    public static class PointHelper
    {
        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

        public static int Area(this Rectangle rect)
        {
            return rect.Width * rect.Height;
        }

        public static int Distance(this Point point, Point p)
        {
            return Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y);
        }

        public static void NormalizePoints(Point[] points, Rectangle rectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            Matrix m = new Matrix();
            m.Translate(rectangle.Center().X, rectangle.Center().Y);

            if (rectangle.Width > rectangle.Height)
                m.Scale(1, 1f * rectangle.Width / rectangle.Height);
            else
                m.Scale(1f * rectangle.Height / rectangle.Width, 1);

            m.Translate(-rectangle.Center().X, -rectangle.Center().Y);
            m.TransformPoints(points);
        }

        public static void NormalizePoints2(Point[] points, Rectangle rectangle, Rectangle needRectangle)
        {
            if (rectangle.Height == 0 || rectangle.Width == 0)
                return;

            float k1 = 1f * needRectangle.Width / rectangle.Width;
            float k2 = 1f * needRectangle.Height / rectangle.Height;
            float k = Math.Min(k1, k2);

            Matrix m = new Matrix();
            m.Scale(k, k);
            m.Translate(needRectangle.X / k - rectangle.X, needRectangle.Y / k - rectangle.Y);
            m.TransformPoints(points);
        }

        public static PointF Offset(this PointF p, float dx, float dy)
        {
            return new PointF(p.X + dx, p.Y + dy);
        }
    }
}
