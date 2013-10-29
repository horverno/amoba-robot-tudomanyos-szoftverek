using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ROBOTIS;

namespace amoba
{
    class Program
    {
        public const int DEFAULT_PORTNUM = 3; // COM3
        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
        const int motorokszama = 7;
        static Mozgas[] mozg = new Mozgas[7];
        static Motor[] motorok = new Motor[motorokszama];
        public static int sleeptime = 0;

        static void Main(string[] args)
        {

            if (dynamixel.dxl_initialize(DEFAULT_PORTNUM, DEFAULT_BAUDNUM) == 0)
            {
                Console.WriteLine("Failed to open USB2Dynamixel!");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey(true);
                return;
            }
            else
            {
                Console.WriteLine("Succeed to open USB2Dynamixel!");
            }


            /*NULL pozicíó begin*/
            int[,] npmov = new int[2, 7];
            npmov[0, 0] = 2;
            npmov[1, 0] = 800;
            npmov[0, 1] = 9;
            npmov[1, 1] = 806;
            npmov[0, 2] = 10;
            npmov[1, 2] = 630;
            npmov[0, 3] = 11;
            npmov[1, 3] = 518;
            npmov[0, 4] = 12;
            npmov[1, 4] = 532;
            npmov[0, 5] = 13;
            npmov[1, 5] = 486;
            //npmov[0,6]=14;
            //npmov[1,6]=325;
            //npmov[0,7]=16;
            //npmov[1,7]=826;
            npmov[0, 6] = 250;
            npmov[1, 6] = 818; //643

            Mozgas np = new Mozgas("NP");
            np.setTomb(npmov);
            /*NULL pozicíó end*/



            /*bábuért megy begin*/
            int[,] bmmov = new int[2, 7];
            bmmov[0, 0] = 2;
            bmmov[1, 0] = 800;
            bmmov[0, 1] = 9;
            bmmov[1, 1] = 190;
            bmmov[0, 2] = 10;
            bmmov[1, 2] = 525;
            bmmov[0, 3] = 11;
            bmmov[1, 3] = 180;
            bmmov[0, 4] = 12;
            bmmov[1, 4] = 689;
            bmmov[0, 5] = 13;
            bmmov[1, 5] = 312;
            //bmmov[0,6]=14;
            //bmmov[1,6]=325;
            //bmmov[0,7]=16;
            //bmmov[1,7]=826;
            bmmov[0,6]=250;
            bmmov[1,6]=900;

            Mozgas bm = new Mozgas("BM");
            bm.setTomb(bmmov);
            /*bábuért megy end*/

            /*bábut markol begin*/
            int[,] bmarkolmov = new int[2, 7];
            bmarkolmov[0, 0] = 2;
            bmarkolmov[1, 0] = 782;
            bmarkolmov[0, 1] = 9;
            bmarkolmov[1, 1] = 217;
            bmarkolmov[0, 2] = 10;
            bmarkolmov[1, 2] = 525;
            bmarkolmov[0, 3] = 11;
            bmarkolmov[1, 3] = 221;
            bmarkolmov[0, 4] = 12;
            bmarkolmov[1, 4] = 616;
            bmarkolmov[0, 5] = 13;
            bmarkolmov[1, 5] = 386;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bmarkolmov[0, 6] = 250;
            bmarkolmov[1, 6] = 900;

            Mozgas bmarkol = new Mozgas("BMM");
            bmarkol.setTomb(bmarkolmov);

            /*bábut markol end*/

            /*bábut markol2 (elmozdítja a felvettet) begin*/
            int[,] bmarkolmovv = new int[2, 7];
           bmarkolmovv[0, 0] = 2;
           bmarkolmovv[1, 0] = 800;
           bmarkolmovv[0, 1] = 9;
           bmarkolmovv[1, 1] = 190;
           bmarkolmovv[0, 2] = 10;
           bmarkolmovv[1, 2] = 525;
           bmarkolmovv[0, 3] = 11;
           bmarkolmovv[1, 3] = 180;
           bmarkolmovv[0, 4] = 12;
           bmarkolmovv[1, 4] = 689;
           bmarkolmovv[0, 5] = 13;
           bmarkolmovv[1, 5] = 312;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bmarkolmovv[0, 6] = 250;
            bmarkolmovv[1, 6] = 818;

            Mozgas bmv = new Mozgas("BMV");
            bmv.setTomb(bmarkolmovv);


            // markolás -> összeszorít
            /*bábut összeszorít begin*/

            int[,] bszorit = new int[2, 7];

            bszorit[0, 0] = 2;
            bszorit[1, 0] = 782;
            bszorit[0, 1] = 9;
            bszorit[1, 1] = 217;
            bszorit[0, 2] = 10;
            bszorit[1, 2] = 525;
            bszorit[0, 3] = 11;
            bszorit[1, 3] = 221;
            bszorit[0, 4] = 12;
            bszorit[1, 4] = 616;
            bszorit[0, 5] = 13;
            bszorit[1, 5] = 386;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bszorit[0, 6] = 250;
            bszorit[1, 6] = 818;

            Mozgas bmszorit = new Mozgas("BMSZ");
            bmszorit.setTomb(bszorit);

            /*bábut összeszorít end*/









            /* mezők begin */


            int[,] a1mov = new int[2, 3];
            a1mov[0,0] = 2;
            a1mov[1,0] = 826;
            a1mov[0,1] = 9;
            a1mov[1,1] = 181;
            a1mov[0, 2] = 10;
            a1mov[1, 2] = 558;

            Mozgas a1 = new Mozgas("A1");
            a1.setTomb(a1mov);

            int[,] a2mov = new int[2, 3];
            a2mov[0, 0] = 250;
            a2mov[1, 0] = 536;
            a2mov[0, 1] = 10;
            a2mov[1, 1] = 622;
            a2mov[0, 2] = 12;
            a2mov[1, 2] = 518;

            Mozgas a2 = new Mozgas("A2");
            a2.setTomb(a2mov);

            /*mezők end*/
            
            /* mozgások aggregálása egy tömbbe */
            mozg[0]=a1;
            mozg[1]=a2;
            mozg[2] = bm;
            mozg[3] = bmarkol;
            mozg[4] = np;
            mozg[5] = bmszorit;
            mozg[6] = bmv;

            /* motorok példányosítása */

            Motor motor1 = new Motor(9);
            Motor motor2 = new Motor(10);
            Motor motor3 = new Motor(11);
            Motor motor4 = new Motor(12);
            Motor motor5 = new Motor(250);

            DuplaMotor motor6 = new DuplaMotor(13, 14);
            DuplaMotor motor7 = new DuplaMotor(2, 16);
            
            motorok[0] = motor1;
            motorok[1] = motor2;
            motorok[2] = motor3;
            motorok[3] = motor4;
            motorok[4] = motor5;
            motorok[5] = motor6;
            motorok[6] = motor7;
          
            //motorok[3] = motor8;
            //Console.WriteLine(motorok[0].getPresentPositon());


            // bekérem, hogy hova mozgassam
            string hova = adatbekeres();


            if (mozgatas("NP"))
            {
                // először fel kell venni a bábut
                if (mozgatas("BM"))
                {
                    if (mozgatas("BMM"))
                    {
                            if (mozgatas("BMSZ"))
                            {
                                if (mozgatas("BMV"))
                                {
                                    if (mozgatas("NP"))
                                    {
                                        dynamixel.dxl_terminate();
                                    }
                                }
                                
                            }
                    }
                }
            }
            dynamixel.dxl_terminate();
        }

        protected static bool mozgatas(string celmezo)
        {
            // a táblán lévő célpozícióba mozgatja
            celmezo = celmezo.ToUpper();
            for (int i = 0; i < mozg.Count(); i++)// mozg-okon megy végig
            {
                if (mozg[i].getCel().Equals(celmezo))   // azt a mozgást keresem ami a bemeneti paraméternek megfelel
                {
                    int[,] akttomb = mozg[i].getTomb();
                    Console.WriteLine(motorok.Count());
                    for (int k = 0; k < motorok.Count(); k++)   // motorokon megy végig
                        {

                            //Console.Write("motorId: ");
                            //Console.WriteLine(motorok[k].getID());
                            //Console.Write("motorid: ");
                            //Console.WriteLine(akttomb[0, k]);

                           
                            for (int f = 0; f < motorok.Count(); f++)
                            {
                                Console.WriteLine(motorok[k].getID() + " " + akttomb[0, f]);
                                if (motorok[k].getID() == akttomb[0, f])
                                {

                                    // elé kell setspeed
                                    motorok[k].setSpeed(50);

                                    Thread.Sleep(100);

                                    motorok[k].Run(akttomb[1, f]);


                                }
                            }
                    }
                }
             
            }
            Console.WriteLine("isready előtt");
            while (!isready()) ;
            return true;
            
        }


        protected static string adatbekeres()
        {
            Console.WriteLine("Add meg a célmezőt");
            return Console.ReadLine();
        }

        protected static bool isready()
        {
            int isOk = 0;
            while (isOk < motorokszama)
            {

                isOk = 0;
                for (int i = 0; i < motorokszama; i++)
                {
                    Console.Write("Motorok száma :" + motorok.Count());

                    if (motorok[i].isInGoalPosition() == true)
                    {
                        isOk++;
                        Console.Write("isOK :");
                        Console.WriteLine(isOk);
                        Console.Write("Motorszám :");
                        Console.WriteLine(i);
                        Thread.Sleep(40);
                    }
                    else
                    {
                        Console.WriteLine("MotorID " + motorok[i].getID());
                        Console.WriteLine("Pozíciója: " + motorok[i].getPresentPositon());
                        Console.WriteLine("Célpozíció: " + motorok[i].getGoalPosition());
                    }
                }

            }
            Console.WriteLine("ready");
            return true;
        }

    }
}
