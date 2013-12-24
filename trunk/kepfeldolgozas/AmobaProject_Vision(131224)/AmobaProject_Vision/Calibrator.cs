using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmobaProject_Vision
{
    class Calibrator
    {

        private List<int[]> values;

        public Calibrator()
        {
            values = new List<int[]>();
        }

        public void AddValue(int x, int y)
        {
            if (values.Count == 10)
            {
                values.RemoveAt(0);
            }
            values.Add(new int[2] { x, y });
        }

        public bool Calibrated()
        {
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i][0] != values[0][0])
                {
                    return false;
                }
                if (values[i][1] != values[0][1])
                {
                    return false;
                }                
            }
            return true;
        }

    }
}
