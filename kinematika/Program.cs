using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ROBOTIS;
using System.Xml;

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
        static Mozgas[] mozg = new Mozgas[11];
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
                        if (mozgatas("BABUFEL"))
                        {
                            if (mozgatas("BABUMEGY"))
                            {
                                if (mozgatas("OSSZEFOG"))
                                {
                                    if (mozgatas("BABUFEL"))
                                    {

                                        if (mozgatas("_VEDDLE"))
                                        {
                                            if (mozgatas("POZ20FEL"))
                                            {
                                                if (mozgatas("POZ30"))
                                                {
                                                    if (mozgatas("POZ30LETESZ"))
                                                    {
                                                        if (mozgatas("POZ30FELNYIT"))
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
            int[,] akttomb = xml_read(celmezo);

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

        protected static int[,] xml_read(string name)
        {

            XmlReader reader = XmlReader.Create("robotkar.xml");
            int[,] akttomb = new int[2, 7];
            int counter = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "mozgas" && reader.GetAttribute(0) == name)
                {
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "motor_poz")
                        {
                            akttomb[0, counter] = Int32.Parse(reader.GetAttribute(0));
                            akttomb[1, counter] = Int32.Parse(reader.GetAttribute(1));
                            counter++;
                        }
                    }
                }
            }
            return akttomb;
        }

    }
}
