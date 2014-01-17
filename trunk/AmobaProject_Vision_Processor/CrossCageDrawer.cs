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
        /// <summary>
        /// A paraméterként kapott kameraképre rárajzolja a detektált négyzethálót a megfelelő dőlésszöggel.
        /// </summary>
        /// <param name="img">Az adott kép, amire rajzol a metódus.</param>
        /// <param name="bigBox">A négyzetháló kerete.</param>
        /// <param name="avgSmallSize">A tábla mezőinek nagysága.</param>
        /// <param name="color">A szín, amilyen színű legyen a kirajzolt négyzetháló.</param>
        /// <returns>A kapott képet ad vissza, a rárajzolt négyzethálóval.</returns>
        public static Image<Bgr, Byte> DrawCage(Image<Bgr, Byte> img, MCvBox2D bigBox, float avgSmallSize, Bgr color)
        {
            Image<Bgr, Byte> result = img.Copy();

            float cageWidth = bigBox.size.Width + 10; //Négyzetháló szélessége.
            float cageHeight = bigBox.size.Height + 10; //Négyzetháló magassága.

            int colNumber = (int)(cageWidth / avgSmallSize); //Oszlopok száma.
            int rowNumber = (int)(cageHeight / avgSmallSize); //Sorok száma.

            result.Draw(bigBox, color, 2); //Először csak maga a négyzetháló kerete kerül a képre.

            if (colNumber > 0 && rowNumber > 0) //Ezt követően a vízszintes és függőleges vonalak is felkerülnek.
            {
                for (int i = 0; i < colNumber - 1; i++) //Függőleges vonalak kirajzolása.
                {
                    result.Draw(GetLine(colNumber, i, false, bigBox), color, 2);
                }

                for (int i = 0; i < rowNumber - 1; i++) //Vízszintes vonalak kirajzolása.
                {
                    result.Draw(GetLine(rowNumber, i, true, bigBox), color, 2);
                }
            }
            
            return result;
        }

        /// <summary>
        /// A négyzetháló vízszintes és függűleges vonalait generáló metódus.
        /// </summary>
        /// <param name="count">Ennyi sor/oszlp-ból áll a négyzetháló.</param>
        /// <param name="n">Ahányadik vonalat akarom kirajzolni a négyzethálón.</param>
        /// <param name="horizontal">Logikai true esetén vízszintes vonalat generál, ellenben függőlegest.</param>
        /// <param name="box">A négyzetháló kerete.</param>
        /// <returns>A paraméterek alapján generált egyenes ad vissza.</returns>
        private static LineSegment2D GetLine(int count, int n, bool horizontal, MCvBox2D box)
        {
            Point lineBeg; //Rajzolandó vonal kezdő koordinátája.
            Point lineEnd; //Rajzolandó vonal végpontjának koordinátája.

            double angle = box.angle; //A vonal dőlésszöge.
            double hypotenuseC; //Háromszögtételhez használatos átfogó.

            if (horizontal)
            {   //Vízszintes vonal esetén.
                 hypotenuseC = box.size.Height;
            }
            else
            {   //Függőleges vonal esetén.
                hypotenuseC = box.size.Width;
            }

            double sideA = Math.Cos(toRadian(angle)) * hypotenuseC; //Háromszögtételhz az A oldal
            double sideB = Math.Sqrt(Math.Pow(hypotenuseC, 2) - Math.Pow(sideA, 2)); //B oldal

            double offX = sideA / count; //Két mező sarokpontja közti vízszintes távolság.
            double offY = sideB / count; //Két mező sarokpontja közti függőleges távolság.

            int cx; //Adott vonal közepének X koordinátája.
            int cy; //Adott vonal közepének Y koordinátája.

            double scaleX; //Kötépponttól scaleX távolságra helyezkedik el vízszintesen a vonal kezdő és végpontja.
            double scaleY; //Kötépponttól scaleY távolságra helyezkedik el függőlegesen a vonal kezdő és végpontja.

            if (horizontal)
            {   //Vízszintes vonal esetén.
                cx = (int)(box.center.X - (((count - 2) / 2.0) - n) * offY);
                cy = (int)(box.center.Y - (((count - 2) / 2.0) - n) * offX);
                scaleX = (Math.Cos(toRadian(angle)) * box.size.Width);
                scaleY = Math.Sqrt(Math.Pow(box.size.Width, 2) - Math.Pow(scaleX, 2));
                lineBeg = new Point((int)(cx + scaleX / 2.0), (int)(cy - scaleY / 2.0));
                lineEnd = new Point((int)(cx - scaleX / 2.0), (int)(cy + scaleY / 2.0));
            }
            else
            {   //Függőleges vonal esetén.
                cx = (int)(box.center.X - (((count - 2) / 2.0) - n) * offX);
                cy = (int)(box.center.Y + (((count - 2) / 2.0) - n) * offY);
                scaleX = (Math.Cos(toRadian(angle)) * box.size.Height);
                scaleY = Math.Sqrt(Math.Pow(box.size.Height, 2) - Math.Pow(scaleX, 2));
                lineBeg = new Point((int)(cx - scaleY / 2.0), (int)(cy - scaleX / 2.0));
                lineEnd = new Point((int)(cx + scaleY / 2.0), (int)(cy + scaleX / 2.0));
            }

            return new LineSegment2D(lineBeg, lineEnd);
        }

        private static double toRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
