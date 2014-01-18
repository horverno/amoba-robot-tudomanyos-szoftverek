using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogicsModule
{
    class Vegpont
    {
        private int sor;
        private int oszlop;

        public Vegpont(int s, int o)
        {
            sor = s;
            oszlop = o;
        }

        public int getSor()
        {
            return sor;
        }
        public int getOszlop()
        {
            return oszlop;
        }
    }
}
