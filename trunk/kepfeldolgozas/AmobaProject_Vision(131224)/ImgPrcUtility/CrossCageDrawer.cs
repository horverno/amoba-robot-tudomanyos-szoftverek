using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV;

namespace AmobaProject_Vision_Processor
{
    public class CrossCageDrawer
    {
        public static Image<Bgr, Byte> DrawCage(Image<Bgr, Byte> img, MCvBox2D bigBox, float avgSmallSize, Bgr color)
        {   //It draws to camera picture the field
            Image<Bgr, Byte> result = img.Copy();

            float cageWidth = bigBox.size.Width + 10;
            float cageHeight = bigBox.size.Height + 10;

            int colNumber = (int)(cageWidth / avgSmallSize);
            int rowNumber = (int)(cageHeight / avgSmallSize);

            result.Draw(bigBox, color, 2);

            //Draw cage
            if (colNumber > 0 && rowNumber > 0)
            {
                for (int i = 0; i < colNumber - 1; i++)
                {
                    result.Draw(GetLineV(colNumber, i, bigBox), color, 2);
                }

                for (int i = 0; i < rowNumber - 1; i++)
                {
                    result.Draw(GetLineH(rowNumber, i, bigBox), color, 2);
                }
            }

            return result;
        }

        private static LineSegment2D GetLineV(int count, int n, MCvBox2D box)
        {   //egy függőleges vonalat ad vissza, a megfelőle pocízióra téve
            //temporary
            LineSegment2D result = new LineSegment2D();
            Point lineBeg;
            Point lineEnd;

            double doloesszog = box.angle;
            double atfogoC = box.size.Width;
            //cosinus tétellel először ezt tudom kiszámolni legegyszerűbben
            //cos(dőlésszög) = (szög melletti befogó)/(átfogó) ebből a befogó kell, átfogó és szög ismert
            double befogoA = Math.Cos(toRadian(doloesszog)) * atfogoC;
            double befogoB = Math.Sqrt(Math.Pow(atfogoC, 2) - Math.Pow(befogoA, 2));

            double offX = befogoA / count;
            double offY = befogoB / count;
            int cx = (int)(box.center.X - (((count - 2) / 2.0) - n) * offX);
            int cy = (int)(box.center.Y + (((count - 2) / 2.0) - n) * offY);

            double leptekX = (Math.Cos(toRadian(doloesszog)) * box.size.Height);
            double leptekY = Math.Sqrt(Math.Pow(box.size.Height, 2) - Math.Pow(leptekX, 2));
            lineBeg = new Point((int)(cx - leptekY / 2.0), (int)(cy - leptekX / 2.0));
            lineEnd = new Point((int)(cx + leptekY / 2.0), (int)(cy + leptekX / 2.0));

            result = new LineSegment2D(lineBeg, lineEnd);

            return result;
        }

        private static LineSegment2D GetLineH(int count, int n, MCvBox2D box)
        {   //egy vízszintes vonalat ad vissza, a megfelőle pocízióra téve
            //temporary
            LineSegment2D result = new LineSegment2D();
            Point lineBeg;
            Point lineEnd;

            double doloesszog = box.angle;
            double atfogoC = box.size.Height;
            //cosinus tétellel először ezt tudom kiszámolni legegyszerűbben
            //cos(dőlésszög) = (szög melletti befogó)/(átfogó) ebből a befogó kell, átfogó és szög ismert
            double befogoA = Math.Cos(toRadian(doloesszog)) * atfogoC;
            double befogoB = Math.Sqrt(Math.Pow(atfogoC, 2) - Math.Pow(befogoA, 2));

            double offX = befogoA / count;
            double offY = befogoB / count;
            int cx = (int)(box.center.X - (((count - 2) / 2.0) - n) * offY);
            int cy = (int)(box.center.Y - (((count - 2) / 2.0) - n) * offX);

            double leptekX = (Math.Cos(toRadian(doloesszog)) * box.size.Width);
            double leptekY = Math.Sqrt(Math.Pow(box.size.Width, 2) - Math.Pow(leptekX, 2));
            lineBeg = new Point((int)(cx + leptekX / 2.0), (int)(cy - leptekY / 2.0));
            lineEnd = new Point((int)(cx - leptekX / 2.0), (int)(cy + leptekY / 2.0));

            result = new LineSegment2D(lineBeg, lineEnd);

            return result;
        }

        public static Image<Bgr, Byte> GameImage(int[,] matrix)
        {   //It draws the game view
            Image<Bgr, Byte> result = new Image<Bgr, byte>(640, 480);
            int col = matrix.GetLength(0);
            int row = matrix.GetLength(1);

            //Boxes size
            int size = 80;
            MCvBox2D bigBox = new MCvBox2D(new PointF(50 + (int)((size * col) / 2), 240), new SizeF(size * col, size * row), 0);
            result.Draw(bigBox, new Bgr(0, 255, 0), 2);

            //Horizontal, Vertical lines
            int line_u = (int)(bigBox.center.Y - bigBox.size.Height / 2);
            int line_d = (int)(bigBox.center.Y + bigBox.size.Height / 2);
            int line_l = (int)(bigBox.center.X - bigBox.size.Width / 2);
            int line_r = (int)(bigBox.center.X + bigBox.size.Width / 2);

            for (int i = 1; i < col; i++)
            {
                LineSegment2D horLine;
                horLine = new LineSegment2D(new Point(line_l + i * size, line_u), new Point(line_l + i * size, line_d));

                result.Draw(horLine, new Bgr(0, 255, 0), 2);
            }

            for (int i = 1; i < row; i++)
            {
                LineSegment2D verLine;
                verLine = new LineSegment2D(new Point(line_l, line_u + i * size), new Point(line_r, line_u + i * size));

                result.Draw(verLine, new Bgr(0, 255, 0), 2);
            }

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                    if (matrix[i, j] == 1)
                    {   //Draw X
                        List<LineSegment2D> x = GameX(new PointF((float)(line_l + (i + 0.5) * size),
                                                                 (float)(line_u + (j + 0.5) * size)), size);
                        foreach (LineSegment2D line in x)
                        {
                            result.Draw(line, new Bgr(255, 0, 0), 2);
                        }
                    }
                    else
                        if (matrix[i, j] == 0)
                        {   //Draw O
                            result.Draw(GameO(new PointF((float)(line_l + (i + 0.5) * size),
                                                         (float)(line_u + (j + 0.5) * size)), size), new Bgr(0, 0, 255), 2);
                        }
                //newnewnew
            }
            return result;
        }

        private static List<LineSegment2D> GameX(PointF center, int size)
        {   //It returns withan X
            int newsize = (int)(size * 0.8);
            List<LineSegment2D> x = new List<LineSegment2D>();
            x.Add(new LineSegment2D(new Point((int)(center.X - newsize / 2), (int)(center.Y - newsize / 2)),
                                    new Point((int)(center.X + newsize / 2), (int)(center.Y + newsize / 2))));

            x.Add(new LineSegment2D(new Point((int)(center.X + newsize / 2), (int)(center.Y - newsize / 2)),
                                    new Point((int)(center.X - newsize / 2), (int)(center.Y + newsize / 2))));
            return x;
        }

        private static CircleF GameO(PointF center, int size)
        {   //It returns with an O
            int newsize = (int)(size * 0.8);
            CircleF o = new CircleF(center, newsize / 2);
            return o;
        }

        private static double toRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
