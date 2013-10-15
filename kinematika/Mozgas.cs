using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace amoba
{
    class Mozgas
    {
        protected string celmezo;
        protected int[,] mozgasok = new int[2, 3];
        
        public Mozgas(string celm)
        {
            this.celmezo = celm;
            
        }

        public void setTomb(int[,] tomb)
        {
            this.mozgasok=tomb;
        }

        public int[,] getTomb()
        {
            return this.mozgasok;
        }

        public string getCel()
        {
            return this.celmezo;
        }
    }
}
