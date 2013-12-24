using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AmobaProject_Vision_Processor
{
    class Calibrator
    {
        private List<int[]> values;
        private int sensitivity;
        private int sarok_X;
        private int sarok_Y;

        public Calibrator(int sensitivity, int sarok_X, int sarok_Y)
        {
            values = new List<int[]>();
            this.sensitivity = sensitivity;
            this.sarok_X = sarok_X;
            this.sarok_Y = sarok_Y;
        }

        public void AddValue(int sor, int oszlop, int x, int y)
        {
            if (values.Count == sensitivity)
            {
                values.RemoveAt(0);
            }
            values.Add(new int[4] {sor, oszlop,  x, y });
        }

        public bool Calibrated()
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i][0] != values[0][0])
                {
                    return false;
                }
                if (values[i][1] != values[0][1])
                {
                    return false;
                }

                //fix sarokponthoz viszonyítunk, hogy a tábla megfelőlő helyen van e
                if (!isHasonlo(values[i][2], sarok_X, 10) || !isHasonlo(values[i][3], sarok_Y, 10))
                    return false;
            }

            return true;
        }

        public Point getSarok(){
            return new Point(sarok_X, sarok_Y);
        }
                                 //95     100      10
        public bool isHasonlo(int n1, int n2, int t){
            //n1 n2+/-t-n belül van-e
            if(n1<n2+t && n1>n2-t)
                return true;
            else
                return false;
        }

    }
}
