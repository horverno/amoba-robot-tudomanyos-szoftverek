using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROBOTIS;
using System.IO;
using System.Threading;

namespace amoba
{
    class Motor
    {
         public const int P_GOAL_POSITION_L = 30;
        public const int P_GOAL_POSITION_H = 31;
        public const int P_PRESENT_POSITION_L = 36;
        public const int P_PRESENT_POSITION_H = 37;
        public const int P_MOVING = 46;
        public const int P_SPEED = 32;


        // Defulat setting
        public const int DEFAULT_PORTNUM = 3; // COM3
        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
        protected int id;
        
        


        
        public Motor(int id)
        {
            this.id = id;
        }

        public virtual void ThreadRun(int pos)
        {
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
        }

        public virtual void setSpeed(int speed){
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
        }

        public int getID()
        {
            return this.id;
        }

        public bool isMoving()
        {
            if (dynamixel.dxl_read_byte(this.id, P_MOVING) != 0)
            {
                return true;
            }
            return false;
        }

        public bool isDoubleEngine()
        {
            if (this.getID() == 13 || this.getID() == 14)
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }
        //getpos

        public int getPresentPositon() 
        {
            return dynamixel.dxl_read_byte(this.id, P_PRESENT_POSITION_L);
        }







        /*
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} ", DateTime.Now.ToString("hh.mm.ss.ffffff"));
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

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
        static void PrintErrorCode()
        {
            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_VOLTAGE) == 1)
                Console.WriteLine("Input voltage error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_ANGLE) == 1)
                Console.WriteLine("Angle limit error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_OVERHEAT) == 1)
                Console.WriteLine("Overheat error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_RANGE) == 1)
                Console.WriteLine("Out of range error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_CHECKSUM) == 1)
                Console.WriteLine("Checksum error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_OVERLOAD) == 1)
                Console.WriteLine("Overload error!");

            if (dynamixel.dxl_get_rxpacket_error(dynamixel.ERRBIT_INSTRUCTION) == 1)
                Console.WriteLine("Instruction code error!");
        }*/

        internal void createThread(int f, int[,] akttomb)
        {
            new Thread(new ThreadStart(delegate()
            {
                System.Threading.Thread.Sleep(Program.sleeptime);
                this.ThreadRun(akttomb[1, f]);
            })).Start();
            Program.sleeptime += 40;
        }
    }
}
