using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AmobaProject_Vision_Processor
{
    class Calibrator
    {
        private List<int[]> values; //Az egyes felismert pályák paraméterei (sor és oszlop szám, x és y koordináta).
        private int sensitivity; //Hány egymás utáni ugyanolyan táblát kell keresni.
        private int cornerX; //Ehhez kell igazítani a pályát.
        private int cornerY; //Ehhez kell igazítani a pályát.

        public Calibrator(int sensitivity, int sarok_X, int sarok_Y) //Konstruktor.
        {
            values = new List<int[]>();
            this.sensitivity = sensitivity;
            this.cornerX = sarok_X;
            this.cornerY = sarok_Y;
        }

        public void AddValue(int sor, int oszlop, int x, int y)
        {
            if (values.Count == sensitivity)
            {
                values.RemoveAt(0);
            }
            values.Add(new int[4] {sor, oszlop,  x, y });
        }

        public bool Calibrated() //Eldönti, hogy a pálya be van e kalibrálva.
        {
            for (int i = 0; i < values.Count; i++)
            {
                //Ha a magasság vagy szélesség nem egyezik, akkor nem jó.
                if (values[i][0] != values[0][0])
                {
                    return false;
                }
                if (values[i][1] != values[0][1])
                {
                    return false;
                }

                //Fix sarokponthoz viszonyítunk, hogy a tábla megfelőlő helyen van e.
                if (!isSimilar(values[i][2], cornerX, 20) || !isSimilar(values[i][3], cornerY, 20))
                    return false;
            }

            return true;
        }

        public Point getCorner(){
            return new Point(cornerX, cornerY);
        }
                               
        public bool isSimilar(int n1, int n2, int t){
            if(n1<n2+t && n1>n2-t)
                return true;
            else
                return false;
        }
    }
}
