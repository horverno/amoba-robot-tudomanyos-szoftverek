using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROBOTIS;
using System.IO;
using System.Threading;

namespace KinematicsModule
{
    /// <summary>
    /// 
    /// </summary>
    class Motor
    {
        public const int P_GOAL_POSITION_L = 30;
        public const int P_GOAL_POSITION_H = 31;
        public const int P_PRESENT_POSITION_L = 36;
        public const int P_PRESENT_POSITION_H = 37;
        public const int P_MOVING = 46;
        public const int P_SPEED = 32;

        public const int P_CW_MARGIN = 26;
        public const int P_CCW_MARGIN = 27;
        public const int P_CW_SLOPE = 28;
        public const int P_CCW_SLOPE = 29;

        // Defulat setting
        public const int DEFAULT_PORTNUM = 3; // COM3
        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
        protected int id;
        protected int goalPosition;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public Motor(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public virtual void Run(int pos)
        {
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
            this.goalPosition = pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed"></param>
        public virtual void setSpeed(int speed)
        {
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
        }


        public virtual void setMargin(int margin)
        {
            dynamixel.dxl_write_word(this.id, P_CW_MARGIN, margin);
            dynamixel.dxl_write_word(this.id, P_CCW_MARGIN, margin);
        }

        public virtual void setSlope(int slope)
        {
            dynamixel.dxl_write_word(this.id, P_CW_SLOPE, slope);
            dynamixel.dxl_write_word(this.id, P_CCW_SLOPE, slope);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getGoalPosition()
        {
            return this.goalPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getID()
        {
            return this.id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isInGoalPosition()
        {

            int margin = 20;
            if (this.id == 250)
            {
                margin = 20;
            }
            if (Math.Abs(dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L) - this.goalPosition) < margin)
            {
                int CommStatus = dynamixel.dxl_get_result();
                if (CommStatus == dynamixel.COMM_RXSUCCESS)
                {
                    return true;
                }
                else
                {
                    PrintCommStatus(CommStatus);
                    return false;
                }
            }

            //Console.WriteLine("\t Nincs pozíción: MotorID: " + this.id + " PresPos: " + dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_H) + " Goalpos: " + this.goalPosition);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getPresentPositon()
        {
            return dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L);
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
        }
    }
}
