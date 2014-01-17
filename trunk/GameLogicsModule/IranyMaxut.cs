using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogicsModule
{
    class IranyMaxut//adott irányba a leghosszabb út
    {
        int mut;
        int sor, oszlop;
        int vegsor, vegoszlop;
        int elozosor, elozooszlop, vegkovsor, vegkovoszlop;

        String irany;

        public IranyMaxut(int ut, String i, int s, int o,int vs,int vo, int es,int eo, int vks,int vko)
        {
            mut = ut;
            sor = s;
            oszlop = o;
            irany = i;
            vegsor = vs;
            vegoszlop = vo;
            elozosor = es;
            elozooszlop = eo;
            vegkovsor = vks;
            vegkovoszlop = vko;

        }
        public int getVegKovSor()
        {
            return vegkovsor;
        }
        public int getVegKovOszlop()
        {
            return vegkovoszlop;
        }
        public int getKezdElozoSor(){
            return elozosor;
        }

        public int getKezdElozoOszlop(){
            return elozooszlop;
        }
        public int getUt(){
            return mut;
        }
        public String getIrany()
        {
            return irany;
        }
        public int getOszlop()
        {
            return oszlop;
        }
        public int getSor()
        {
            return sor;
        }

        public int getVegSor()
        {
            return vegsor;
        }
        public int getVegOszlop()
        {
            return vegoszlop;
        }

        public String getUtAdat()
        {
            String s;
            s = "Irány: " + irany + " Hossz: "+ mut + " Kezdőpont: " + sor + ";" + oszlop+" Vegpont: "+vegsor+";"+vegoszlop +" Ut előtti pont:" + elozosor +";"+ elozooszlop+" Ut utani pont:" + vegkovsor+ ";" +vegkovoszlop;
            return s;
        }
    }
}
