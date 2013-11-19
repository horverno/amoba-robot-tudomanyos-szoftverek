using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ROBOTIS;

namespace amoba
{
    // <summary>
    // 
    // </summary>

    class Program
    {
        public const int DEFAULT_PORTNUM = 3; // COM3
        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
        const int motorokszama = 7;
        static Mozgas[] mozg = new Mozgas[10];
        static Motor[] motorok = new Motor[motorokszama];
        public static int sleeptime = 0;

        // <summary>
        // 
        // </summary>
        // <param name="args"></param>
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



            int[,] teszt4mov = new int[2, 7];
            teszt4mov[0, 0] = 2;
            teszt4mov[1, 0] = 666;
            teszt4mov[0, 1] = 9;
            teszt4mov[1, 1] = 557;
            teszt4mov[0, 2] = 10;
            teszt4mov[1, 2] = 1023;
            teszt4mov[0, 3] = 11;
            teszt4mov[1, 3] = 171;
            teszt4mov[0, 4] = 12;
            teszt4mov[1, 4] = 650;
            teszt4mov[0, 5] = 13;
            teszt4mov[1, 5] = 524;
            teszt4mov[0, 6] = 250;
            teszt4mov[1, 6] = 823;
            Mozgas teszt4 = new Mozgas("TESZT4");
            teszt4.setTomb(teszt4mov);

            int[,] babumegymov = new int[2, 7];
            babumegymov[0, 0] = 2;
            babumegymov[1, 0] = 801;
            babumegymov[0, 1] = 9;
            babumegymov[1, 1] = 854;
            babumegymov[0, 2] = 10;
            babumegymov[1, 2] = 811;
            babumegymov[0, 3] = 11;
            babumegymov[1, 3] = 233;
            babumegymov[0, 4] = 12;
            babumegymov[1, 4] = 638;
            babumegymov[0, 5] = 13;
            babumegymov[1, 5] = 636;
            babumegymov[0, 6] = 250;
            babumegymov[1, 6] = 866;
            Mozgas babumegy = new Mozgas("BABUMEGY");
            babumegy.setTomb(babumegymov);

            int[,] osszefogmov = new int[2, 7];
            osszefogmov[0, 0] = 2;
            osszefogmov[1, 0] = 801;
            osszefogmov[0, 1] = 9;
            osszefogmov[1, 1] = 854;
            osszefogmov[0, 2] = 10;
            osszefogmov[1, 2] = 811;
            osszefogmov[0, 3] = 11;
            osszefogmov[1, 3] = 233;
            osszefogmov[0, 4] = 12;
            osszefogmov[1, 4] = 638;
            osszefogmov[0, 5] = 13;
            osszefogmov[1, 5] = 636;
            osszefogmov[0, 6] = 250;
            osszefogmov[1, 6] = 813;
            Mozgas osszefog = new Mozgas("OSSZEFOG");
            osszefog.setTomb(osszefogmov);

            int[,] elengedmov = new int[2, 7];
            elengedmov[0, 0] = 2;
            elengedmov[1, 0] = 781;
            elengedmov[0, 1] = 9;
            elengedmov[1, 1] = 761;
            elengedmov[0, 2] = 10;
            elengedmov[1, 2] = 250;
            elengedmov[0, 3] = 11;
            elengedmov[1, 3] = 316;
            elengedmov[0, 4] = 12;
            elengedmov[1, 4] = 686;
            elengedmov[0, 5] = 13;
            elengedmov[1, 5] = 666;
            elengedmov[0, 6] = 250;
            elengedmov[1, 6] = 823;
            Mozgas elenged = new Mozgas("ELENGED");
            elenged.setTomb(elengedmov);

            int[,] konyokBemov = new int[2,7];
            konyokBemov[0, 0] = 2;
            konyokBemov[1, 0] = 800;
            konyokBemov[0, 1] = 9;
            konyokBemov[1, 1] = 806;
            konyokBemov[0, 2] = 10;
            konyokBemov[1, 2] = 630;
            konyokBemov[0, 3] = 11;
            konyokBemov[1, 3] = 518;
            konyokBemov[0, 4] = 12;
            konyokBemov[1, 4] = 803;
            konyokBemov[0, 5] = 13;
            konyokBemov[1, 5] = 486;
            konyokBemov[0, 6] = 250;
            konyokBemov[1, 6] = 818;
            Mozgas konyokBe = new Mozgas("KONYOKBE");
            konyokBe.setTomb(konyokBemov);


            


            int[,] teszt3mov = new int[2, 7];
            teszt3mov[0, 0] = 2;
            teszt3mov[1, 0] = 705;
            teszt3mov[0, 1] = 9;
            teszt3mov[1, 1] = 598;
            teszt3mov[0, 2] = 10;
            teszt3mov[1, 2] = 1023;
            teszt3mov[0, 3] = 11;
            teszt3mov[1, 3] = 192;
            teszt3mov[0, 4] = 12;
            teszt3mov[1, 4] = 614;
            teszt3mov[0, 5] = 13;
            teszt3mov[1, 5] = 522;
            teszt3mov[0, 6] = 250;
            teszt3mov[1, 6] = 823;
            Mozgas teszt3 = new Mozgas("TESZT3");
            teszt3.setTomb(teszt3mov);


            int[,] teszt2mov = new int[2, 7];
            teszt2mov[0, 0] = 2;
            teszt2mov[1, 0] = 827;
            teszt2mov[0, 1] = 9;
            teszt2mov[1, 1] = 869;
            teszt2mov[0, 2] = 10;
            teszt2mov[1, 2] = 600;
            teszt2mov[0, 3] = 11;
            teszt2mov[1, 3] = 562;
            teszt2mov[0, 4] = 12;
            teszt2mov[1, 4] = 657;
            teszt2mov[0, 5] = 13;
            teszt2mov[1, 5] = 660;
            teszt2mov[0, 6] = 250;
            teszt2mov[1, 6] = 823;
            Mozgas teszt2 = new Mozgas("TESZT2");
            teszt2.setTomb(teszt2mov);


            int[,] teszt1mov = new int[2, 7];
            teszt1mov[0, 0] = 2;
            teszt1mov[1, 0] = 781;
            teszt1mov[0, 1] = 9;
            teszt1mov[1, 1] = 761;
            teszt1mov[0, 2] = 10;
            teszt1mov[1, 2] = 250;
            teszt1mov[0, 3] = 11;
            teszt1mov[1, 3] = 316;
            teszt1mov[0, 4] = 12;
            teszt1mov[1, 4] = 686;
            teszt1mov[0, 5] = 13;
            teszt1mov[1, 5] = 666;
            teszt1mov[0, 6] = 250;
            teszt1mov[1, 6] = 813;
            Mozgas teszt1 = new Mozgas("TESZT1");
            teszt1.setTomb(teszt1mov);

            //pihenőből bábúért

            int[,] _veddlemov = new int[2, 7];
            _veddlemov[0, 0] = 2;
            _veddlemov[1, 0] = 800;
            _veddlemov[0, 1] = 9;
            _veddlemov[1, 1] = 828;
            _veddlemov[0, 2] = 10;
            _veddlemov[1, 2] = 428;
            _veddlemov[0, 3] = 11;
            _veddlemov[1, 3] = 442;
            _veddlemov[0, 4] = 12;
            _veddlemov[1, 4] = 834;
            _veddlemov[0, 5] = 13;
            _veddlemov[1, 5] = 831;
            _veddlemov[0, 6] = 250;
            _veddlemov[1, 6] = 813;
            Mozgas _veddle = new Mozgas("_VEDDLE");
            _veddle.setTomb(_veddlemov);



            int[,] _00mov = new int[2, 7];
            _00mov[0, 0] = 2;
            _00mov[1, 0] = 839;
            _00mov[0, 1] = 9;
            _00mov[1, 1] = 842;
            _00mov[0, 2] = 10;
            _00mov[1, 2] = 0;
            _00mov[0, 3] = 11;
            _00mov[1, 3] = 416;
            _00mov[0, 4] = 12;
            _00mov[1, 4] = 647;
            _00mov[0, 5] = 13;
            _00mov[1, 5] = 631;
            _00mov[0, 6] = 250;
            _00mov[1, 6] = 824;
            Mozgas _00 = new Mozgas("_00");
            _00.setTomb(_00mov);

            int[,] _10mov = new int[2, 7];
            _10mov[0, 0] = 2;
            _10mov[1, 0] = 841;
            _10mov[0, 1] = 9;
            _10mov[1, 1] = 870;
            _10mov[0, 2] = 10;
            _10mov[1, 2] = 0;
            _10mov[0, 3] = 11;
            _10mov[1, 3] = 514;
            _10mov[0, 4] = 12;
            _10mov[1, 4] = 646;
            _10mov[0, 5] = 13;
            _10mov[1, 5] = 636;
            _10mov[0, 6] = 250;
            _10mov[1, 6] = 824;
            Mozgas _10 = new Mozgas("_10");
            _10.setTomb(_10mov);

            int[,] _20mov = new int[2, 7];
            _20mov[0, 0] = 2;
            _20mov[1, 0] = 837;
            _20mov[0, 1] = 9;
            _20mov[1, 1] = 871;
            _20mov[0, 2] = 10;
            _20mov[1, 2] = 0;
            _20mov[0, 3] = 11;
            _20mov[1, 3] = 574;
            _20mov[0, 4] = 12;
            _20mov[1, 4] = 642;
            _20mov[0, 5] = 13;
            _20mov[1, 5] = 646;
            _20mov[0, 6] = 250;
            _20mov[1, 6] = 829;
            Mozgas _20 = new Mozgas("_20");
            _20.setTomb(_20mov);

            int[,] _30mov = new int[2, 7];
            _30mov[0, 0] = 2;
            _30mov[1, 0] = 812;
            _30mov[0, 1] = 9;
            _30mov[1, 1] = 819;
            _30mov[0, 2] = 10;
            _30mov[1, 2] = 1023;
            _30mov[0, 3] = 11;
            _30mov[1, 3] = 580;
            _30mov[0, 4] = 12;
            _30mov[1, 4] = 643;
            _30mov[0, 5] = 13;
            _30mov[1, 5] = 660;
            _30mov[0, 6] = 250;
            _30mov[1, 6] = 822;
            Mozgas _30 = new Mozgas("_30");
            _30.setTomb(_30mov);

            int[,] _01mov = new int[2, 7];
            _01mov[0, 0] = 2;
            _01mov[1, 0] = 805;
            _01mov[0, 1] = 9;
            _01mov[1, 1] = 861;
            _01mov[0, 2] = 10;
            _01mov[1, 2] = 77;
            _01mov[0, 3] = 11;
            _01mov[1, 3] = 371;
            _01mov[0, 4] = 12;
            _01mov[1, 4] = 623;
            _01mov[0, 5] = 13;
            _01mov[1, 5] = 631;
            _01mov[0, 6] = 250;
            _01mov[1, 6] = 822;
            Mozgas _01 = new Mozgas("_01");
            _01.setTomb(_01mov);

            int[,] _11mov = new int[2, 7];
            _11mov[0, 0] = 2;
            _11mov[1, 0] = 807;
            _11mov[0, 1] = 9;
            _11mov[1, 1] = 845;
            _11mov[0, 2] = 10;
            _11mov[1, 2] = 1001;
            _11mov[0, 3] = 11;
            _11mov[1, 3] = 400;
            _11mov[0, 4] = 12;
            _11mov[1, 4] = 643;
            _11mov[0, 5] = 13;
            _11mov[1, 5] = 642;
            _11mov[0, 6] = 250;
            _11mov[1, 6] = 825;
            Mozgas _11 = new Mozgas("_11");
            _11.setTomb(_11mov);

            int[,] _21mov = new int[2, 7];
            _21mov[0, 0] = 2;
            _21mov[1, 0] = 792;
            _21mov[0, 1] = 9;
            _21mov[1, 1] = 797;
            _21mov[0, 2] = 10;
            _21mov[1, 2] = 958;
            _21mov[0, 3] = 11;
            _21mov[1, 3] = 455;
            _21mov[0, 4] = 12;
            _21mov[1, 4] = 624;
            _21mov[0, 5] = 13;
            _21mov[1, 5] = 637;
            _21mov[0, 6] = 250;
            _21mov[1, 6] = 825;
            Mozgas _21 = new Mozgas("_21");
            _21.setTomb(_21mov);

            int[,] _31mov = new int[2, 7];
            _31mov[0, 0] = 2;
            _31mov[1, 0] = 750;
            _31mov[0, 1] = 9;
            _31mov[1, 1] = 717;
            _31mov[0, 2] = 10;
            _31mov[1, 2] = 1023;
            _31mov[0, 3] = 11;
            _31mov[1, 3] = 447;
            _31mov[0, 4] = 12;
            _31mov[1, 4] = 643;
            _31mov[0, 5] = 13;
            _31mov[1, 5] = 630;
            _31mov[0, 6] = 250;
            _31mov[1, 6] = 828;
            Mozgas _31 = new Mozgas("_31");
            _31.setTomb(_31mov);




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
            bmmov[0, 6] = 250;
            bmmov[1, 6] = 900;

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
            a1mov[0, 0] = 2;
            a1mov[1, 0] = 826;
            a1mov[0, 1] = 9;
            a1mov[1, 1] = 181;
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

            mozg[0] = np;
            
            mozg[1] = _veddle;
            mozg[2] = babumegy;
            mozg[3] = _11;
            mozg[4] = _11;
            mozg[5] = _20;
            mozg[6] = teszt1;
            mozg[7] = konyokBe;
            mozg[8] = osszefog;
            mozg[9] = elenged;




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

            //sebessegbeallitas(motorok, "NP");
            if (mozgatas("NP"))
            {
                // először fel kell venni a bábut
                if (mozgatas("KONYOKBE"))
                {
                    if (mozgatas("_VEDDLE"))
                    {
                        if (mozgatas("BABUMEGY"))
                        {
                            if (mozgatas("OSSZEFOG"))
                            {
                                if (mozgatas("_VEDDLE"))
                                {
                                    if (mozgatas("TESZT1"))
                                    {
                                        if (mozgatas("ELENGED"))
                                        {
                                            if (mozgatas("_VEDDLE"))
                                            {
                                                if (mozgatas("KONYOKBE"))
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
                                motorok[k].setSpeed(25);

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
        protected static void sebessegbeallitas(Motor[] motorok, string hova)// motorok t�mb �s a c�lmez�(pl.: a1)
        {
            int[,] celok;
            int[,] elmozdulasok = new int[0, 0];
            int[,] akttomb;
            int[,] sebessegek;      // id �s sebess�gp�rosokat t�rol
            int hossz = 0;
            for (int j = 0; j < mozg.Length; j++)
            {
                if (mozg[j].getCel() == hova)   // azt a mozg�st keresem ami a bemeneti param�ternek megfelel
                {
                    akttomb = mozg[j].getTomb();
                    hossz = akttomb.Length / 2;
                    celok = new int[2, hossz];
                    elmozdulasok = new int[2, hossz];
                    for (int k = 0; k < hossz; k++)
                    {
                        celok[0, k] = akttomb[0, j];        // az ID-ket is t�rolom hozz�
                        celok[1, k] = akttomb[1, j];       // t�rolja a c�lokat sz�pen sorban
                        elmozdulasok[1, k] = Math.Abs(celok[1, k] - motorok[k].getPresentPositon());        // a konkr�t elmozdul�sok nagys�ga
                    }
                }
            }

            sebessegek = new int[2, hossz];

            // miut�n megvannak az elmozdul�sok elmozdulasok[2, motorokszama]


            int maxelm = 0;
            int maxid = 250;
            int maxspeed = 50;
            for (int i = 0; i < hossz; i++)
            {
                if (elmozdulasok[1, i] > maxelm)
                {
                    maxelm = elmozdulasok[1, i];
                    maxid = elmozdulasok[0, i];
                }
            }
            // itt m�r megvan a legnagyobb elmozdul�s �s az ahhoz tartoz� motorID
            int oszto = maxelm / maxspeed;      // kiszamolas: speed=elm/oszto
            for (int z = 0; z < hossz; z++)
            {
                sebessegek[0, z] = elmozdulasok[0, z];  // ID �tad�sa
                sebessegek[1, z] = elmozdulasok[1, z] / oszto;
                motorok[z].setSpeed(elmozdulasok[1, z] / oszto);        // be�ll�tom a sebess�get
            }
        }

    }
}
