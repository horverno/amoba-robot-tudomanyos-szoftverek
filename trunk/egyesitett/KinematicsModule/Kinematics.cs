using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterfaceModule;
using ROBOTIS;
using System.Threading;
using System.Xml;

namespace KinematicsModule
{
    public class Kinematics : IKinematics
    {
        public event EventHandler<RobotStatusChangedEventArgs> RobotStatusChanged;
        public event EventHandler<PostMessageEventArgs> PostMessageShowRequest;

        public const int DEFAULT_PORTNUM = 3; // COM3
        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
        const int motorokszama = 7;
        static Mozgas[] mozg = new Mozgas[11];
        static Motor[] motorok = new Motor[motorokszama];
        public static int sleeptime = 0;
        private RobotStatus robotStatus;
             
        public RobotStatus status {
            get
            {
                return robotStatus;
            }
            private set
            {
                robotStatus = value;
                OnRobotStatusChanged(status);
            }
        }

        public Kinematics()
        {
            if (dynamixel.dxl_initialize(DEFAULT_PORTNUM, DEFAULT_BAUDNUM) == 0)
            {
                OnPostMessageShowRequest("Failed to open USB2Dynamixel!");
                //Console.WriteLine("Press any key to terminate...");
                //Console.ReadKey(true);
                status = RobotStatus.Offline;
            }
            else
            {
                OnPostMessageShowRequest("Succeeded to open USB2Dynamixel!");
                status = RobotStatus.Ready;
            }

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

            // bekérem, hogy hova mozgassam
            //string hova = adatbekeres();

            //int wasitok = aktmozgas(0, 0);
            //if (wasitok == 0)
            //{
            //    //Console.ReadKey();
            //}
        }
        /// <summary>This method is for request the UI to show a message</summary>
        /// <param name="message">the message to be shown</param>
        public void OnPostMessageShowRequest(string message)
        {
            if (PostMessageShowRequest != null)
                PostMessageShowRequest(this, new PostMessageEventArgs(message));
        }
        /// <summary>This method must be called when the status of the robot arm changes</summary>
        /// <param name="newStatus">the new status of the robot</param>
        public void OnRobotStatusChanged(RobotStatus newStatus)
        {
            if (RobotStatusChanged != null)
                RobotStatusChanged(this, new RobotStatusChangedEventArgs(newStatus));
        }
        /// <param name="e">this object contains all information about the requested movement</param>
        public void RobotMovementRequestHandler(object sender, RobotMovementRequestEventArgs e)
        {
            switch (e.movementType)
            {
                case RobotMovement.CleanUp: // tábla leszedés
                    break;

                case RobotMovement.PlacePiece:  // bábu felhelyezés
                    status = RobotStatus.Moving;
                    OnPostMessageShowRequest("movement (" + e.ToString() + ") started...");
                    if (aktmozgas(e.destRow, e.destCol) == 1)   // TODO: new thread / async?
                    {
                        status = RobotStatus.Ready;
                    }
                    else
                    {
                        status = RobotStatus.ServoError;
                    }
                    OnPostMessageShowRequest("movement stopped"); 
                    break;

                case RobotMovement.Cheer:   // nyertes mozdulat
                    break;

                case RobotMovement.Grieve:  // vesztes mozdulat
                    break;
            }
            
            
        }

        //////////////////////////////////////////////
        public static int aktmozgas(int x, int y)
        {
            string akt;
            akt = "POZ" + x + y;

            if (mozgatas("NP"))
            {
                if (mozgatas("KONYOKBE"))
                {
                    if (mozgatas("KONYOKBEFEL"))
                    {
                        if (mozgatas("BABUFELE"))
                        {
                            if (mozgatas("BABUMELLETT"))
                            {
                                if (mozgatas("BABUFELETT"))
                                {
                                    if (mozgatas("BABUOSSZEFOG"))
                                    {
                                        if (mozgatas("BABUELVESZ"))
                                        {
                                            if (mozgatas("VEDDLEZARVA"))
                                            {
                                                if (mozgatas(akt + "FELETT"))
                                                {
                                                    if (mozgatas(akt + "ELENGED"))
                                                    {
                                                        if (mozgatas(akt + "ELENGEDFELETT"))
                                                        {
                                                            if (mozgatas("VEDDLENYITVA")) // pálya fölé miután letette
                                                            {
                                                                if (mozgatas("null"))
                                                                {
                                                                    if (mozgatas("KONYOKBEFEL"))
                                                                    {
                                                                        if (mozgatas("KONYOKBE"))
                                                                        {
                                                                            if (mozgatas("NP"))
                                                                            {
                                                                                //Királyság
                                                                                return 1;
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
                }
            }

            return 0;
        }


        protected static bool mozgatas(string celmezo)
        {
            // a táblán lévő célpozícióba mozgatja
            celmezo = celmezo.ToUpper();
            int[,] akttomb = xml_read(celmezo);
            if (akttomb[0, 0] == -1)
            {
                return false;
            }


            for (int k = 0; k < motorok.Count(); k++)   // motorokon megy végig
            {

                ////Console.Write("motorId: ");
                ////Console.WriteLine(motorok[k].getID());
                ////Console.Write("motorid: ");
                ////Console.WriteLine(akttomb[0, k]);


                for (int f = 0; f < motorok.Count(); f++)
                {
                    //Console.WriteLine(motorok[k].getID() + " " + akttomb[0, f]);
                    if (motorok[k].getID() == akttomb[0, f])
                    {

                        // elé kell setspeed
                        motorok[k].setSpeed(50);

                        Thread.Sleep(50);

                        //motorok[k].setMargin(1); // int 1-254
                        //Thread.Sleep(50);

                        //motorok[k].setSlope(8); //csak kettő hatványai 128-ig
                        //Thread.Sleep(50);

                        motorok[k].Run(akttomb[1, f]);


                    }
                }
            }



            //Console.WriteLine("isready előtt");
            while (!isready()) ;
            return true;

        }


        protected static string adatbekeres()
        {
            //Console.WriteLine("Add meg a célmezőt");
            //return Console.ReadLine();
            return null;
        }

        protected static bool isready()
        {
            int isOk = 0;
            while (isOk < motorokszama)
            {

                isOk = 0;
                for (int i = 0; i < motorokszama; i++)
                {
                    //Console.Write("Motorok száma :" + motorok.Count());

                    if (motorok[i].isInGoalPosition() == true)
                    {
                        isOk++;
                        //Console.Write("isOK :");
                        //Console.WriteLine(isOk);
                        //Console.Write("Motorszám :");
                        //Console.WriteLine(i);
                        Thread.Sleep(40);
                    }
                    else
                    {
                        //Console.WriteLine("MotorID " + motorok[i].getID());
                        //Console.WriteLine("Pozíciója: " + motorok[i].getPresentPositon());
                        //Console.WriteLine("Célpozíció: " + motorok[i].getGoalPosition());
                    }
                }

            }
            //Console.WriteLine("ready");
            return true;
        }



        static void sebessegbeallitas(Motor[] motorok, string hova)// motorok t�mb �s a c�lmez�(pl.: a1)
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
            XmlReader reader = null;
            int[,] akttomb = new int[2, 7];
            try
            {
                reader = XmlReader.Create("robotkar.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                akttomb[0, 0] = -1;
                akttomb[0, 1] = -1;
                return akttomb;
            }

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




        // --------------------------------------------------------------------------------------------------

        // Print communication result
        static void PrintCommStatus(int CommStatus)
        {
            switch (CommStatus)
            {
                case dynamixel.COMM_TXFAIL:
                    Console.WriteLine("COMM_TXFAIL: Failed transmit instruction packet!");
                    break;

                case dynamixel.COMM_TXERROR:
                    Console.WriteLine("COMM_TXERROR: Incorrect instruction packet!");
                    break;

                case dynamixel.COMM_RXFAIL:
                    Console.WriteLine("COMM_RXFAIL: Failed get status packet from device!");
                    break;

                case dynamixel.COMM_RXWAITING:
                    Console.WriteLine("COMM_RXWAITING: Now recieving status packet!");
                    break;

                case dynamixel.COMM_RXTIMEOUT:
                    Console.WriteLine("COMM_RXTIMEOUT: There is no status packet!");
                    break;

                case dynamixel.COMM_RXCORRUPT:
                    Console.WriteLine("COMM_RXCORRUPT: Incorrect status packet!");
                    break;

                default:
                    Console.WriteLine("This is unknown error code!");
                    break;
            }
        }

        // Print error bit of status packet
        void PrintErrorCode()
        {
            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_VOLTAGE) == 1)
                //Console.WriteLine("Input voltage error!");
                 OnPostMessageShowRequest("Input voltage error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_ANGLE) == 1)
                //Console.WriteLine("Angle limit error!");
                OnPostMessageShowRequest("Angle limit error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_OVERHEAT) == 1)
                //Console.WriteLine("Overheat error!");
                OnPostMessageShowRequest("Overheat error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_RANGE) == 1)
                //Console.WriteLine("Out of range error!");
                OnPostMessageShowRequest("Out of range error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_CHECKSUM) == 1)
                //Console.WriteLine("Checksum error!");
                OnPostMessageShowRequest("Checksum error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_OVERLOAD) == 1)
                //Console.WriteLine("Overload error!");
                OnPostMessageShowRequest("Overload error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_INSTRUCTION) == 1)
                //Console.WriteLine("Instruction code error!");
                OnPostMessageShowRequest("Instruction code error!");
        }

    }
}
