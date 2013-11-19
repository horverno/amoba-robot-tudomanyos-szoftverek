using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace amoba
{
    /// <summary>
    /// 
    /// </summary>
    class Mozgas
    {
        protected string celmezo;
        protected int[,] mozgasok = new int[2, 7];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="celm"></param>
        public Mozgas(string celm)
        {
            this.celmezo = celm;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tomb"></param>
        public void setTomb(int[,] tomb)
        {
            this.mozgasok=tomb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[,] getTomb()
        {
            return this.mozgasok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getCel()
        {
            return this.celmezo;
        }
    }
}
