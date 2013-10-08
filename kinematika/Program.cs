﻿using System;
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
        const int motorokszama = 2;
        static Mozgas[] mozg = new Mozgas[2];
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
            
            int[,] a1mov = new int[2, 2];
            a1mov[0,0] = 250;
            a1mov[1,0] = 870;
            a1mov[0,1] = 10;
            a1mov[1,1] = 800;

            Mozgas a1 = new Mozgas("A1");
            a1.setTomb(a1mov);

            int[,] a2mov = new int[2, 2];
            a2mov[0, 0] = 250;
            a2mov[1, 0] = 800;
            a2mov[0, 1] = 10;
            a2mov[1, 1] = 750;

            Mozgas a2 = new Mozgas("A2");
            a2.setTomb(a2mov);

            
            mozg[0]=a1;
            mozg[1]=a2;


            Motor motor1 = new Motor(250); // 870
            Motor motor2 = new Motor(10);  // 800
            Motor motor3 = new Motor(12);  // 700

            DuplaMotor motor8 = new DuplaMotor(13, 14); // 800

            
            motorok[0] = motor1;
            motorok[1] = motor2;
            //motorok[2] = motor3;
            //motorok[3] = motor8;

            
            // bekérem, hogy hova mozgassam
            string hova = adatbekeres();


            // először fel kell venni a bábut
            //mozgatas("babuert_menni");
            if (isready())
            {
                sleeptime = 0;
                // melyik mezőre mozgassam
                mozgatas(hova);
                if (isready())
                {
                    sleeptime = 0;
                    // már mind leállt
                    //mozgatas("babut_ledob");
                    if (isready())
                    {
                        sleeptime = 0;
                        // most kell ledobni a bábut, aztán visszamozgatni pihenőbe
                        //mozgatas("pihenobe");
                        if (isready())
                        {
                            sleeptime = 0;
                            // vezérlés a képfelismerésnek
                        }
                    }
                    Console.WriteLine("megált");
                    Console.ReadKey();
                    dynamixel.dxl_terminate();
                }

            }
            
        }

        protected static void mozgatas(string celmezo)
        {
            // a táblán lévő célpozícióba mozgatja
            celmezo = celmezo.ToUpper();
            for (int i = 0; i < mozg.Count(); i++)// mozg-okon megy végig
            {
                if (mozg[i].getCel().Equals(celmezo))   // azt a mozgást keresem ami a bemeneti paraméternek megfelel
                {
                    int[,] akttomb = mozg[i].getTomb();
                    for (int k = 0; k < motorok.Count(); k++)   // motorokon megy végig
                    {
                        for (int f = 0; f < 2; f++)  // motorokra lebontott mozgást tartalmazó tömbbön végigmegyünk
                        {
                            Console.Write("motorId: ");
                            Console.WriteLine(motorok[k].getID());
                            Console.Write("motorId2: ");
                            Console.WriteLine(akttomb[0, k]);

                            if (motorok[k].getID() == akttomb[0, k])
                            {
                                // elé kell setspeed
                                motorok[k].setSpeed(50);
                                /*Console.Write("f: ");
                                Console.WriteLine(f);*/

                                motorok[k].createThread(f, akttomb);

                                
                                
                            }
                            
                        }
                    }
                }
            }
            
        }


        protected static string adatbekeres()
        {
            Console.WriteLine("Add meg a célmezőt");
            return Console.ReadLine();
        }

        protected static bool isready()
        {
            bool mozog = true;
            while (mozog)   // vizsgálom, hogy mozog e még valamelyik motor
            {
                mozog = false;
                for (int i = 0; i < motorok.Count(); i++)
                {
                    if (motorok[i].isMoving())
                    {
                        mozog = false;
                    }
                }
            }
            return true;
        }

    }
}
