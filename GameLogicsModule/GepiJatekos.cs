using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameLogicsModule
{
    public class GepiJatekos
    {
        //Adott iranyban a maximalis utak
        List<IranyMaxut> maxutirany = new List<IranyMaxut>(); 
        //Aktuális leghosszabb út, maximumkereséshez szükséges változó
        List<IranyMaxut> maxutiranyAfterPasztazX = new List<IranyMaxut>();
        //Aktuális leghosszabb út, maximumkereséshez szükséges változó: X
        List<IranyMaxut> maxutiranyAfterPasztazO = new List<IranyMaxut>();
        //Aktuális leghosszabb út, maximumkereséshez szükséges változó: KÖR


        private int maxut;
        private int sor;
        private int oszlop;
        int longestX = int.MinValue;
        int longestO = int.MinValue;
        Boolean treeLongExist=false;
        Boolean isTableEmpty = false;
        int db = 0;
        int nextStepX = -1000;
        int nextStepY = -1000;
        private String msg = "";
        
        List<IranyMaxut> veszelyKeres(int[,] jt, int s, int o, int babu)
        {
            msg = "";
            maxut = 2;
            maxutirany.Clear();
            int[,] jatekter = new int[s, o];
            jatekter = jt;
            List<IranyMaxut> message=new List<IranyMaxut>();
           
            int i;
            int j;
            sor = s-1;
            oszlop = o-1;
            message.Clear();
            //Minden ponton végigmegyünk és meghívjuk a maxutirany kereső fv-t
            for (i = 0; i <sor; i++)
            {
                for (j = 0; j <oszlop; j++)
                {
                    
                    message=pasztaz(jatekter, i, j, sor-1, oszlop-1,babu);

                }
                
            }
            //Kivesszük ugyanazokat a maxutakat pl.: 1;1 -> 3;3 | 3;3->1;1
            message=redundancia(message);

            //Kiírja a leghosszabb utat
            foreach (IranyMaxut x in message)
            {
                //MessageBox.Show(x.getUtAdat());
                msg = msg + x.getUtAdat()+"\r\n";
            }
            //message = maxutirany;
            
            return message;
        }

        public String fourPiecesSearch(int[,] jt, int s, int o, int babu)
        {
            List<IranyMaxut> temp = new List<IranyMaxut>();
            maxut = 2;
            maxutirany.Clear();
            int[,] jatekter = new int[s, o];
            jatekter = jt;

            int i;
            int j;
            sor = s - 1;
            oszlop = o - 1;
            String message="default";

            //Minden ponton végigmegyünk és meghívjuk a maxutirany kereső fv-t
            for (i = 0; i < sor; i++)
            {
                for (j = 0; j < oszlop; j++)
                {

                    temp = pasztazAll(jatekter, i, j, sor - 1, oszlop - 1, babu);

                }

            }
            //Kivesszük ugyanazokat a maxutakat pl.: 1;1 -> 3;3 | 3;3->1;1
            temp = redundancia(temp);

            //Kiírja a leghosszabb utat
            foreach (IranyMaxut x in temp)
            {
                if (x.getUt() == 4)
                {
                    if (babu == 1)
                    {
                        //MessageBox.Show("X nyert");
                        message = "X nyert!";
                    }
                    else
                    {
                        //MessageBox.Show("O nyert");
                        message = "O nyert!";
                    }
                }
            }


            return message;
        }



        int BadPointDelete(IranyMaxut m){

            Boolean okFirst = true;
            Boolean okSecond = true;

            if ((m.getKezdElozoSor() < 0) || (m.getKezdElozoSor() > sor)) 
                {okFirst=false; }

            else
                { if ((m.getKezdElozoOszlop() < 0) || (m.getKezdElozoOszlop() > oszlop)) { okFirst = false; } }


            if ((m.getVegKovSor () < 0) || (m.getVegKovSor () > sor))
            { okSecond = false; }

            else
            { if ((m.getVegKovOszlop () < 0) || (m.getVegKovOszlop () > oszlop)) { okSecond = false; } }


            if (okFirst == true) { return 1; }
            else { return 2; }

        }

        public int [] nextStepGen(int[,] jatekter, int s, int o)
        {
            int[] nextStep = new int[2];
            int i, j;       
            sor = s - 1;
            oszlop = o - 1;
            longestX = int.MinValue;
            longestO = int.MinValue;
            String message;
            String message2;
            List<IranyMaxut> message3 = new List<IranyMaxut>();
            List<IranyMaxut> message4 = new List<IranyMaxut>();
                     

            //1. lépés: Ha valakinek 4 sszimbóluma van:-----------------------------------------------------------------

                  
           message=fourPiecesSearch(jatekter, 5, 5,1);
           
           message2=fourPiecesSearch(jatekter, 5, 5, 0);

           if (message == "X nyert!") { /*MessageBox.Show("X nyert");*/ nextStep[0] = -20; nextStep[1] = -20; }
           else if (message2 == "O nyert!") { 
               /*MessageBox.Show("O nyert");*/ nextStep[0] = -10; nextStep[1] = -10; }                
               //2. lépés: - megnézzük van-e 3 hosszú út , mert akkor oda kell tenni------------------------------------------
            
           else
           { 

               message3=veszelyKeres(jatekter, 5, 5, 0);
               message4=veszelyKeres(jatekter, 5, 5, 1);

               db = 0;
               //3. lépés: A tábla üres.
               for (i = 0; i < sor; i++)
               {
                   for (j = 0; j < oszlop; j++)
                   {
                       if (jatekter[i, j] != 2) { db++;}
                   }
               }
               if (db == 0) { isTableEmpty = true; /*MessageBox.Show("Üres tábla"); */}

               if (isTableEmpty == true)
               {
                   Random rnd2 = new Random();
                   nextStepX = rnd2.Next(0, 4);
                   nextStepY = rnd2.Next(0, 4);
                   while (jatekter[nextStepX, nextStepY] != 2)
                   {
                       Random rnd = new Random();
                       int x = rnd.Next(0, 4);
                       if (x == 0) { nextStepX = 1; nextStepY = 1; }
                       if (x == 1) { nextStepX = 1; nextStepY = 2; }
                       if (x == 2) { nextStepX = 2; nextStepY = 1; }
                       if (x == 3) { nextStepX = 2; nextStepY = 2; }
                   }
                   msg = msg + "Üres tábla! X:" + nextStepY + " - " + "Y: " + nextStepY + "\r\n";
                   //MessageBox.Show(nextStepX.ToString() + ";" + nextStepY.ToString());
                   nextStep[0]=nextStepX;
                   nextStep[1]=nextStepY;
               }                        
               
                //if (message3.Count() == 0 && message4.Count() == 0) { /*Üres a tábla*/}

               else if (message3.Count() != 0 && message4.Count() != 0)
               {
                   
                   if (message3[0].getUt() == 3)
                   {
                       if (BadPointDelete(message3[0]) == 1)
                       {
                           //MessageBox.Show(message3[0].getKezdElozoSor().ToString() + " - " + message3[0].getKezdElozoOszlop().ToString()); 


                           nextStep[0] = message3[0].getKezdElozoSor();
                           nextStep[1] = message3[0].getKezdElozoOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                       else
                       {
                           //MessageBox.Show(message3[0].getVegKovSor().ToString() + " - " + message3[0].getVegKovOszlop().ToString()); 
                           nextStep[0] = message3[0].getVegKovSor();
                           nextStep[1] = message3[0].getVegKovOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                   }

                   else if (message4[0].getUt() == 3)
                   {
                       
                       if (BadPointDelete(message4[0]) == 1)
                       {
                           //MessageBox.Show(message4[0].getKezdElozoSor().ToString() + " - " + message4[0].getKezdElozoOszlop().ToString()); 
                           nextStep[0] = message4[0].getKezdElozoSor();
                           nextStep[1] = message4[0].getKezdElozoOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                       else
                       {
                           //MessageBox.Show(message4[0].getVegKovSor().ToString() + " - " + message4[0].getVegKovOszlop().ToString()); 
                           nextStep[0] = message4[0].getVegKovSor();
                           nextStep[1] = message4[0].getVegKovOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                   }

                   //3. Algoritmus eredménye,ha 2 hosszú sorok vannak: ----------------------------------------------------------------------
                   else
                   {
                       //MessageBox.Show("1-2 hosszu sorok vannak.");

                       int k, l;
                       sor = s - 1;
                       oszlop = o - 1;
                       int longestXX = int.MinValue;
                       int longestOO = int.MinValue;
                       int XnextStepX = -1000;
                       int XnextStepY = -1000;
                       int OnextStepX = -1000;
                       int OnextStepY = -1000;
                       int nextStepX = -1000;
                       int nextStepY = -1000;



                       for (i = 0; i < sor; i++)
                       {
                           for (j = 0; j < oszlop; j++)
                           {

                               if (jatekter[i, j] == 2)
                               {
                                   jatekter[i, j] = 1; //ahol üres ott X-re állítja, majd lefut a pasztaz fgv. 


                                   for (k = 0; k < sor; k++)
                                   {
                                       for (l = 0; l < oszlop; l++)
                                       {

                                           maxutiranyAfterPasztazX = pasztaz(jatekter, k, l, sor - 1, oszlop - 1, 1);
                                       }
                                   }
                                   if (maxutiranyAfterPasztazX.Count == 0)
                                   {                                                                              
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   else
                                   {
                                       if (maxutiranyAfterPasztazX[0].getUt() > longestO)
                                       {
                                           longestXX = maxutiranyAfterPasztazX[0].getUt();
                                           XnextStepX = i;
                                           XnextStepY = j;
                                       }
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   jatekter[i, j] = 0; //ahol üres ott KÖR-re állítja, majd lefut a pasztaz fgv.

                                   for (k = 0; k < sor; k++)
                                   {
                                       for (l = 0; l < oszlop; l++)
                                       {
                                           maxutiranyAfterPasztazO = pasztaz(jatekter, k, l, sor - 1, oszlop - 1, 0);
                                       }
                                   }
                                   if (maxutiranyAfterPasztazO.Count == 0)
                                   {                                       
                                       //throw new InvalidOperationException("Empty list");
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   else
                                   {
                                       if (maxutiranyAfterPasztazO[0].getUt() > longestO)
                                       {
                                           longestOO = maxutiranyAfterPasztazO[0].getUt();
                                           OnextStepX = i;
                                           OnextStepY = j;
                                       }

                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre

                                   }

                               }

                           }
                       }

                       String jatekos = "";
                       if (longestXX < longestOO)
                       {

                           nextStepX = OnextStepX;
                           nextStepY = OnextStepY;
                           jatekos = "KÖRJátékos";


                       }
                       else //HA az ellenfél a KÖR-rel van ÉS ő kezdett ÉS hosszabb "sora" lenne a lépés után oda teszünk 1 X-et
                       {
                           nextStepX = XnextStepX;
                           nextStepY = XnextStepY;
                           jatekos = "Xjatekos";

                       }

                       //MessageBox.Show(nextStepX.ToString() + " X, " + nextStepY.ToString() + "Y" + "jatekos_tipus:" + jatekos);
                       nextStep[0] = nextStepX;
                       nextStep[1] = nextStepY;
                       msg = msg + "2 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";

                   }
               }
               else if (message3.Count() != 0 && message4.Count() == 0)
               {

                       if (BadPointDelete(message3[0]) == 1)
                       {
                           //MessageBox.Show(message3[0].getKezdElozoSor().ToString() + " - " + message3[0].getKezdElozoOszlop().ToString()); 

                           nextStep[0] = message3[0].getKezdElozoSor();
                           nextStep[1] = message3[0].getKezdElozoOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                       else
                       {
                           //MessageBox.Show(message3[0].getVegKovSor().ToString() + " - " + message3[0].getVegKovOszlop().ToString()); 
                           nextStep[0] = message3[0].getVegKovSor();
                           nextStep[1] = message3[0].getVegKovOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       
                   }

               }
               else if (message3.Count() == 0 && message4.Count() != 0)
               {

                       if (BadPointDelete(message4[0]) == 1)
                       {
                           //MessageBox.Show(message4[0].getKezdElozoSor().ToString() + " - " + message4[0].getKezdElozoOszlop().ToString()); 
                           nextStep[0] = message4[0].getKezdElozoSor();
                           nextStep[1] = message4[0].getKezdElozoOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                       else
                       {
                           //MessageBox.Show(message4[0].getVegKovSor().ToString() + " - " + message4[0].getVegKovOszlop().ToString()); 
                           nextStep[0] = message4[0].getVegKovSor();
                           nextStep[1] = message4[0].getVegKovOszlop();
                           msg = msg + "3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                       }
                   
               }
               else
               {
                   //MessageBox.Show("1-2 hosszu sorok vannak.");

                   int k, l;
                   sor = s - 1;
                   oszlop = o - 1;
                   int longestXX = int.MinValue;
                   int longestOO = int.MinValue;
                   int XnextStepX = -1000;
                   int XnextStepY = -1000;
                   int OnextStepX = -1000;
                   int OnextStepY = -1000;
                   int nextStepX = -1000;
                   int nextStepY = -1000;
                   int darab = 0;

                   for (i = 0; i < sor; i++)
                   {
                       for (j = 0; j < oszlop; j++)
                       {
                           if (jatekter[i, j] == 0)
                           {
                               darab++;
                           }
                       }
                   }

                   if (darab == 0)
                   {
                      Random rnd2 = new Random();
                      nextStepX = rnd2.Next(0, 4);
                      nextStepY = rnd2.Next(0, 4);

                      while(jatekter[nextStepX,nextStepY]!=2){
                       Random rnd = new Random();
                       int x = rnd.Next(0, 4);
                       if (x == 0) { nextStepX = 1; nextStepY = 1; }
                       if (x == 1) { nextStepX = 1; nextStepY = 2; }
                       if (x == 2) { nextStepX = 2; nextStepY = 1; }
                       if (x == 3) { nextStepX = 2; nextStepY = 2; }
                      }
                       msg = msg + "O még nincs lenn:" + nextStepY + " - " + "Y: " + nextStepY + "\r\n";
                       //MessageBox.Show(nextStepX.ToString() + ";" + nextStepY.ToString());
                       nextStep[0] = nextStepX;
                       nextStep[1] = nextStepY;
                   }
                   else
                   {

                       for (i = 0; i < sor; i++)
                       {
                           for (j = 0; j < oszlop; j++)
                           {

                               if (jatekter[i, j] == 2)
                               {
                                   jatekter[i, j] = 1; //ahol üres ott X-re állítja, majd lefut a pasztaz fgv. 


                                   for (k = 0; k < sor; k++)
                                   {
                                       for (l = 0; l < oszlop; l++)
                                       {

                                           maxutiranyAfterPasztazX = pasztaz(jatekter, k, l, sor - 1, oszlop - 1, 1);
                                       }
                                   }
                                   if (maxutiranyAfterPasztazX.Count == 0)
                                   {
                                       
                                       //throw new InvalidOperationException("Empty list");
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   else
                                   {
                                       if (maxutiranyAfterPasztazX[0].getUt() > longestO)
                                       {
                                           longestXX = maxutiranyAfterPasztazX[0].getUt();
                                           XnextStepX = i;
                                           XnextStepY = j;
                                       }
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   jatekter[i, j] = 0; //ahol üres ott KÖR-re állítja, majd lefut a pasztaz fgv.

                                   for (k = 0; k < sor; k++)
                                   {
                                       for (l = 0; l < oszlop; l++)
                                       {
                                           maxutiranyAfterPasztazO = pasztaz(jatekter, k, l, sor - 1, oszlop - 1, 0);
                                       }
                                   }
                                   if (maxutiranyAfterPasztazO.Count == 0)
                                   {
                                       
                                       //throw new InvalidOperationException("Empty list");
                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre
                                   }
                                   else
                                   {
                                       if (maxutiranyAfterPasztazO[0].getUt() > longestO)
                                       {
                                           longestOO = maxutiranyAfterPasztazO[0].getUt();
                                           OnextStepX = i;
                                           OnextStepY = j;
                                       }

                                       jatekter[i, j] = 2;      //pasztaz után visszaállítja üresre

                                   }

                               }

                           }
                       }

                       String jatekos = "";
                       if (longestXX < longestOO)
                       {

                           nextStepX = OnextStepX;
                           nextStepY = OnextStepY;
                           jatekos = "KÖRJátékos";

                       }
                       else //HA az ellenfél a KÖR-rel van ÉS ő kezdett ÉS hosszabb "sora" lenne a lépés után oda teszünk 1 X-et
                       {
                           nextStepX = XnextStepX;
                           nextStepY = XnextStepY;
                           jatekos = "Xjatekos";

                       }

                       //MessageBox.Show(nextStepX.ToString() + " X, " + nextStepY.ToString() + "Y" + "jatekos_tipus:" + jatekos);
                       nextStep[0] = nextStepX;
                       nextStep[1] = nextStepY;
                       msg = msg + "\nNem üres a tábla, nincs 3 hosszú sor! - X:" + nextStep[0] + " - " + "Y: " + nextStep[1] + "\r\n";
                   }
               }
           }

           if (nextStep[0] >= sor ||  nextStep[1] >= oszlop)
           {

                   Random rnd2 = new Random();
                   nextStep[0] = rnd2.Next(0, 4);
                   nextStep[1] = rnd2.Next(0, 4);

                   while (jatekter[nextStep[0], nextStep[1]] != 2)
                   {
                       Random rnd = new Random();
                       int x = rnd.Next(0, 4);
                       int y = rnd.Next(0, 4);
                       nextStep[0] = x;
                       nextStep[1] = y;
                   }
               
           }
           return nextStep;
        }

        // <--András


        List<IranyMaxut> pasztaz(int[,] jt, int ik, int jk, int sor, int oszlop, int pontfajta)
        {

            int utdb = 0;
            int i;
            int j;
            int vegponti;
            int vegpontj;


            //BALFEL
            i = ik;
            j = jk;


            while (i >= 0 && j >= 0 && jt[i, j] == pontfajta)
            {
                utdb++;
                i--;
                j--;
            }

            vegponti = ik - (utdb - 1);
            vegpontj = jk - (utdb - 1);

            
            if (maxut < utdb)
            {      
                IranyMaxut im = new IranyMaxut(utdb, "BALFEL", ik, jk, vegponti, vegpontj, ik + 1, jk + 1, vegponti - 1, vegpontj - 1);

                
                
                    if (SzabadMaxutUj(im, jt))
                    {
                        maxutirany.Clear();
                        maxut = utdb;
                        maxutirany.Add(im);
                    }
                

            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "BALFEL", ik, jk, vegponti, vegpontj, ik + 1, jk + 1, vegponti - 1, vegpontj - 1);

               
                if (SzabadMaxutUj(im, jt))
                {
                    maxutirany.Add(im);
                }

            }
            //BALRA
            utdb = 0;
            i = ik;
            j = jk;


            while (j >= 0 && jt[i, j] == pontfajta)
            {

                utdb++;
                j--;

            }
            vegponti = ik;
            vegpontj = jk - (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "BAL", ik, jk, vegponti, vegpontj, ik, jk + 1, vegponti, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);
                }

            }
            else if (maxut == utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "BAL", ik, jk, vegponti, vegpontj, ik, jk + 1, vegponti, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "BAL", ik, jk, vegponti, vegpontj, ik, jk + 1, vegponti, vegpontj + 1));
                }
            }
            //JOBBRA
            utdb = 0;
            i = ik;
            j = jk;

            while (j <= oszlop && jt[i, j] == pontfajta)
            {

                utdb++;

                j++;

            }
            vegponti = ik;
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "JOBBRA", ik, jk, vegponti, vegpontj, ik, jk - 1, vegponti, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);                    
                }

            }
            else if (maxut == utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "JOBBRA", ik, jk, vegponti, vegpontj, ik, jk - 1, vegponti, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "JOBBRA", ik, jk, vegponti, vegpontj, ik, jk - 1, vegponti, vegpontj + 1));                   
                }
            }


            //FEL
            utdb = 0;
            i = ik;
            j = jk;


            while (i >= 0 && jt[i, j] == pontfajta)
            {

                utdb++;

                i--;
            }

            vegponti = ik - (utdb - 1);
            vegpontj = jk;

            if (maxut < utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "FEL", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                if(SzabadMaxutUj(im,jt)){
                maxut = utdb;
                maxutirany.Clear();
                maxutirany.Add(im);
                }
            }
            else if (maxut == utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "FEL", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "FEL", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj));
                }
            }

            //JOBBFEL
            utdb = 0;
            i = ik;
            j = jk;

            while (i >= 0 && j <= oszlop && jt[i, j] == pontfajta)
            {

                utdb++;

                i--;
                j++;
            }
            vegponti = ik - (utdb - 1);
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im= new IranyMaxut(utdb, "JOBBFEL", ik, jk, vegponti, vegpontj, ik + 1, jk - 1, vegponti - 1, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                maxut = utdb;
                maxutirany.Clear();
                maxutirany.Add(im);
                }

            }
            else if (maxut == utdb)
            {
                IranyMaxut im= new IranyMaxut(utdb, "JOBBFEL", ik, jk, vegponti, vegpontj, ik + 1, jk - 1, vegponti - 1, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "JOBBFEL", ik, jk, vegponti, vegpontj, ik + 1, jk - 1, vegponti - 1, vegpontj + 1));
                }
            }

            //BALLE
            utdb = 0;
            i = ik;
            j = jk;

            while (i <= sor && j >= 0 && jt[i, j] == pontfajta)
            {

                utdb++;
                i++;
                j--;
            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk - (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "BALLE", ik, jk, vegponti, vegpontj, ik - 1, jk + 1, vegponti + 1, vegpontj - 1);
                if(SzabadMaxutUj(im,jt)){
                maxut = utdb;
                maxutirany.Clear();
                maxutirany.Add(im);
                }

            }
            else if (maxut == utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "BALLE", ik, jk, vegponti, vegpontj, ik - 1, jk + 1, vegponti + 1, vegpontj - 1);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "BALLE", ik, jk, vegponti, vegpontj, ik - 1, jk + 1, vegponti + 1, vegpontj - 1));
                }
            }
            //LE
            utdb = 0;
            i = ik;
            j = jk;

            while (i <= sor && jt[i, j] == pontfajta)
            {

                utdb++;

                i++;

            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk;

            if (maxut < utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "LE", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                if(SzabadMaxutUj(im,jt)){
                maxut = utdb;
                maxutirany.Clear();
                maxutirany.Add(im);
                }

            }
            else if (maxut == utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "LE", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                if(SzabadMaxutUj(im,jt)){
                    maxutirany.Add(new IranyMaxut(utdb, "LE", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj));
                }
            }
            //JOBBLE
            utdb = 0;
            i = ik;
            j = jk;

            while (i <= sor && j <= oszlop && jt[i, j] == pontfajta)
            {

                utdb++;

                i++;
                j++;
            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im=new IranyMaxut(utdb, "JOBBLE", ik, jk, vegponti, vegpontj, ik - 1, jk - 1, vegponti + 1, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                maxut = utdb;
                maxutirany.Clear();
                maxutirany.Add(im);
                }

            }
            else if (maxut == utdb)
            {
                                IranyMaxut im=new IranyMaxut(utdb, "JOBBLE", ik, jk, vegponti, vegpontj, ik - 1, jk - 1, vegponti + 1, vegpontj + 1);
                if(SzabadMaxutUj(im,jt)){
                maxutirany.Add(new IranyMaxut(utdb, "JOBBLE", ik, jk, vegponti, vegpontj, ik - 1, jk - 1, vegponti + 1, vegpontj + 1));
                }
            }
            
            return maxutirany;
        }
        
        List<IranyMaxut> pasztazAll(int[,] jt, int ik, int jk, int sor, int oszlop,int pontfajta)
        {
            
            int utdb = 0;
            int i;
            int j;
            int vegponti;
            int vegpontj;

        
            //BALFEL
            i = ik;
            j = jk;


            while (i >= 0 && j >= 0 && jt[i, j] == pontfajta)
            {
                    utdb++;
                    i--;
                    j--;
            }  
            
            vegponti = ik - (utdb - 1);
            vegpontj = jk - (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "BALFEL", ik, jk, vegponti, vegpontj, ik + 1, jk + 1, vegponti - 1, vegpontj - 1);

                
                        maxutirany.Clear();
                        maxut = utdb;
                        maxutirany.Add(im);
                        
                    

            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "BALFEL", ik, jk, vegponti, vegpontj, ik + 1, jk + 1, vegponti - 1, vegpontj - 1);

                
                
                    maxutirany.Add(im);
                 
                
            }
            //BALRA
            utdb = 0;
            i = ik;
            j = jk;


            while (j >=0 && jt[i, j] == pontfajta){                
                
                 utdb++;
                j--;

            } 
            vegponti = ik;
            vegpontj = jk - (utdb - 1);

            if (maxut < utdb)
            {
                    IranyMaxut im = new IranyMaxut(utdb, "BAL", ik, jk, vegponti, vegpontj, ik, jk + 1, vegponti, vegpontj + 1);
                   
                        maxut = utdb;
                        maxutirany.Clear();
                        maxutirany.Add(im);
                    

            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "BAL", ik, jk, vegponti, vegpontj, ik, jk + 1, vegponti, vegpontj + 1);
               
                    maxutirany.Add(im);
                
                
            }
            //JOBBRA
            utdb = 0;
            i = ik;
            j = jk;

            while (j <=oszlop && jt[i, j] == pontfajta)
            {

                    utdb++;
         
                j++;

            }
            vegponti = ik;
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "JOBBRA", ik, jk, vegponti, vegpontj, ik, jk - 1, vegponti, vegpontj + 1);
                
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);
                

            }
            else if (maxut == utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "JOBBRA", ik, jk, vegponti, vegpontj, ik, jk - 1, vegponti, vegpontj + 1);
              
                    maxutirany.Add(im);
                   
                
            }


            //FEL
            utdb = 0;
            i = ik;
            j = jk;


            while (i >= 0 && jt[i, j] == pontfajta)
            {

                    utdb++;
                
                i--;
            }

            vegponti = ik - (utdb - 1);
            vegpontj = jk;

            if (maxut < utdb )
            {
                IranyMaxut im = new IranyMaxut(utdb, "FEL", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);
                 
            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "FEL", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);
                
                
                    maxutirany.Add(im);
                  
                
            }

            //JOBBFEL
            utdb = 0;
            i = ik;
            j = jk;

            while (i >=0 && j <=oszlop && jt[i, j] == pontfajta)
            {

                    utdb++;
               
                i--;
                j++;
            }
            vegponti = ik - (utdb - 1);
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "JOBBFEL", ik, jk, vegponti, vegpontj, ik + 1, jk - 1, vegponti - 1, vegpontj + 1);
                
                
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);
                
               
            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "JOBBFEL", ik, jk, vegponti, vegpontj, ik + 1, jk - 1, vegponti - 1, vegpontj + 1);
                
                
                    maxutirany.Add(im);
                
                
            }

            //BALLE
            utdb = 0;
            i = ik;
            j = jk;

            while (i <=sor && j>=0 && jt[i, j] == pontfajta)
            {

                    utdb++;
                i++;
                j--;
            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk - (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "BALLE", ik, jk, vegponti, vegpontj, ik - 1, jk + 1, vegponti + 1, vegpontj - 1);
               
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);
                
                
            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "BALLE", ik, jk, vegponti, vegpontj, ik - 1, jk + 1, vegponti + 1, vegpontj - 1);
               
                    maxutirany.Add(im);
                
                
            }
            //LE
            utdb = 0;
            i = ik;
            j = jk;

            while (i <=sor && jt[i, j] == pontfajta)    //FIXIT: túlindexelés előfordulhat
            {

                    utdb++;
                
                i++;

            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk ;

            if (maxut < utdb)
            {
                
                IranyMaxut im = new IranyMaxut(utdb, "LE", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);

               
                    
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);

                
            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "LE", ik, jk, vegponti, vegpontj, ik - 1, jk, vegponti + 1, vegpontj);

               
                    
                    maxutirany.Add(im);

                
                
            }
            //JOBBLE
            utdb = 0;
            i = ik;
            j = jk;
            
            while (i <=sor && j <=oszlop && jt[i, j] == pontfajta)
            {

                    utdb++;
                
                i++;
                j++;
            }

            vegponti = ik + (utdb - 1);
            vegpontj = jk + (utdb - 1);

            if (maxut < utdb)
            {
                IranyMaxut im = new IranyMaxut(utdb, "JOBBLE", ik, jk, vegponti, vegpontj, ik - 1, jk - 1, vegponti + 1, vegpontj + 1);
               
                    maxut = utdb;
                    maxutirany.Clear();
                    maxutirany.Add(im);

                
            }
            else if (maxut == utdb)
            {

                IranyMaxut im = new IranyMaxut(utdb, "JOBBLE", ik, jk, vegponti, vegpontj, ik - 1, jk - 1, vegponti + 1, vegpontj + 1);
                
                
                    maxutirany.Add(im);

                
                
            }
            
            return maxutirany;
        }

        public Boolean vegponte(int vi,int vj, int sor,int oszlop){
	        Boolean vegpont=false;

	        List<Vegpont> vegpontok= new List<Vegpont>();
	        int i,j;
	
	        for(i=0;i<sor;i++){
		        if(i==0){
			        for(j=0;j<oszlop;j++){
				
				        Vegpont v=new Vegpont(i,j);
				        vegpontok.Add(v);
                        
                    }

		        }
		        else if (i==sor-1){
		
			        for(j=0;j<oszlop;j++){
				
				        Vegpont v=new Vegpont(i,j);
				        vegpontok.Add(v);
			        }
		        }
		        else{
			        Vegpont v1=new Vegpont(i,0);
				        vegpontok.Add(v1);
			        Vegpont v2=new Vegpont(i,oszlop-1);
				        vegpontok.Add(v2);
		        }
	        }

	        foreach(Vegpont v in vegpontok){
		        if(v.getSor()==vi && v.getOszlop()==vj){
			        vegpont=true;
		        }
	        }
            
	        return vegpont;		

        }



        List<IranyMaxut> redundancia(List<IranyMaxut> mx)
        {
            List<IranyMaxut> mutiranymasolat = new List<IranyMaxut>();
            List<IranyMaxut> torlendo = new List<IranyMaxut>();
            IranyMaxut max=new IranyMaxut(0,"",0,0,0,0,0,0,0,0);

            foreach (IranyMaxut y in mx)
            {
                mutiranymasolat.Add(y);
            }

            for (int i = mx.Count - 1; i >=0; i--)
            {
                for (int j = 0; j <= mutiranymasolat.Count-1; j++)
                {
                    try
                    {
                        if (mx[i].getSor() == mutiranymasolat[j].getVegSor() && mx[i].getOszlop() == mutiranymasolat[j].getVegOszlop() && mx[i].getVegSor() == mutiranymasolat[j].getSor() && mx[i].getVegOszlop() == mutiranymasolat[j].getOszlop())
                        {

                            
                            torlendo.Add(mx[i]);
                        }
                        else
                        {

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    for (int x =1;x<=torlendo.Count - 1; x++)
                    {
                        mutiranymasolat.Remove(torlendo[x]);
                    }
                }
            }
            return mutiranymasolat;
        }

               
         //////Faltól-falig érő utak kiszűrse, hogy ne kerülhessen be a listába, ha nincs szabad hely az út előtt vagy után

        Boolean SzabadMaxutUj(IranyMaxut im, int[,] jt)
        {
            Boolean szabad = false;
            
            if (im.getKezdElozoSor() < 0 || im.getKezdElozoSor() >= sor || im.getKezdElozoOszlop()<0 || im.getKezdElozoOszlop()>=oszlop)//Ha nem létezik a kezdőpont előtti
            {
                //MessageBox.Show("előtte nem létezik"+im.getUtAdat());
                if(im.getVegKovOszlop()<0 || im.getVegKovOszlop()>=oszlop || im.getVegKovSor()<0 || im.getVegKovSor()>=sor){//Ha nem létezik a végpont utáni

                    szabad=false;//Biztos, hogy nem szabad a vége
                }
                else{// Ha létezik a végpont utáni akkor...
                    if (jt[im.getVegKovSor(), im.getVegKovOszlop()] == 2)
                    { // Akkor, ha az szabad a pont akkor....
                        szabad = true;
                    }
                    else// Különben ...
                    {
                        szabad = false;
                    }
                }
            }
            else if(im.getVegKovOszlop()<0 || im.getVegKovOszlop()>=oszlop || im.getVegKovSor()<0 || im.getVegKovSor()>=sor)// különben, ha a végpont utáni nem létezik,
            {
                //MessageBox.Show("utána nem létezik"+im.getUtAdat());
                if (im.getKezdElozoSor() < 0 || im.getKezdElozoSor() >= sor || im.getKezdElozoOszlop() < 0 || im.getKezdElozoOszlop() >= oszlop) //...és a kezdőpont előtti sem létezik, akkor...
                {
                    szabad = false;
                }
                else//Különben létezik a ...
                {
                    if (jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] == 2)//Ha a kezdőpont előtti szabad, akkor ok
                    {
                        szabad = true;
                    }
                    else//különben nem szabad
                    {
                        szabad = false;
                    }
                }
            }
            else if (im.getVegKovOszlop() >= 0 && im.getVegKovOszlop() < oszlop && im.getVegKovSor() >= 0 && im.getVegKovSor() < sor && im.getKezdElozoSor() >= 0 && im.getKezdElozoSor() < sor && im.getKezdElozoOszlop() >= 0 && im.getKezdElozoOszlop() < oszlop) //Ha mindkettő létező pont, akkor...
            {
                //MessageBox.Show("mind2 létezik"+im.getUtAdat());
                if (jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] == 2 || jt[im.getVegKovSor(), im.getVegOszlop()] == 2)// ha legalább az egyik szabad, akkor ok
                {
                    szabad = true;
                }
                else// különben nem ok
                {
                    szabad = false;
                }
            }
            else// ha egyik pont sem létezik, akkor, biztos, h nem is szabad, így biztos, hogy NEM ok
            {
                //MessageBox.Show("egyiksem létezik"+im.getUtAdat());
                szabad = false;
            }

            return szabad;
        }


        /*public Boolean SzabadMaxut(IranyMaxut im , int[,] jt){
	        Boolean szabad=false;

            if (!vegponte(im.getVegSor(), im.getVegOszlop(), sor, oszlop) && vegponte(im.getSor(), im.getOszlop(), sor, oszlop))
            {
                
		        if(im.getVegKovSor()>=0 && im.getVegKovSor()<sor && im.getVegKovOszlop()<oszlop && im.getVegKovOszlop()>=0){ // nincs túlindexelés
                    
                    if (jt[im.getVegKovSor(), im.getVegKovOszlop()] == 2)
                    {
                        
				        szabad=true;

			        }

		        }

	        }
            else if (vegponte(im.getVegSor(), im.getVegOszlop(), sor, oszlop) && !vegponte(im.getSor(), im.getOszlop(), sor, oszlop))
            {
		        if(im.getKezdElozoSor()>=0 && im.getKezdElozoSor()<sor && im.getKezdElozoOszlop()<oszlop && im.getKezdElozoOszlop()>=0){ // nincs túlindexelés
			
			        if(jt[im.getKezdElozoSor(),im.getKezdElozoOszlop()] == 2){
				        szabad=true;

			        }

		        }


	        }
            else if (!vegponte(im.getVegSor(), im.getVegOszlop(), sor, oszlop) && !vegponte(im.getSor(), im.getOszlop(), sor, oszlop))
            {
                if (im.getKezdElozoSor() >= 0 && im.getKezdElozoSor() < sor && im.getKezdElozoOszlop() < oszlop && im.getKezdElozoOszlop() >= 0 && im.getVegKovSor() >= 0 && im.getVegKovSor() < sor && im.getVegKovOszlop() < oszlop && im.getVegKovOszlop() >= 0)
                { // nincs túlindexelés

                    if (jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] == 2 && jt[im.getVegKovSor(), im.getVegKovOszlop()] != 2)
                    {
                        szabad = true;

                    }

                    else if (jt[im.getVegKovSor(), im.getVegKovOszlop()] == 2 && jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] != 2)
                    {
                        szabad = true;

                    }
                    else if (jt[im.getVegKovSor(), im.getVegKovOszlop()] == 2 && jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] == 2)
                    {
                        szabad = true;
                    }

                    else if (jt[im.getVegKovSor(), im.getVegKovOszlop()] != 2 && jt[im.getKezdElozoSor(), im.getKezdElozoOszlop()] != 2)
                    {
                        szabad = false;
                    }
                }
              else if (vegponte(im.getVegSor(), im.getVegOszlop(), sor, oszlop) && vegponte(im.getSor(), im.getOszlop(), sor, oszlop))
              {
                  if (im.getOszlop() <= 0)
                  {
                      if(jt[im.getVegKovOszlop(),im.getVegKovSor()]!=2){

                      }
                  }

              }


            }

	        return szabad;

        }*/

        public String getMsg()
        {
            return msg;
        }

        public Boolean dontetlen(int[,] jatekter)
        {
            Boolean d = false;
            int i;
            int j;
            int uresszama = 0;

            for (i = 0; i < sor; i++)
            {
                for (j = 0; j < oszlop; j++)
                {
                    if (jatekter[i, j] == 2)
                    {
                        uresszama++;
                    }
                }
            }
            if (uresszama == 0)
            {
                d = true;
            }
            return d;
        }

        
    }

}


