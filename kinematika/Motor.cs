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
        protected int goalPosition;
        
        


        
        public Motor(int id)
        {
            this.id = id;
        }

        public virtual void Run(int pos)
        {
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
            this.goalPosition = pos;
        }

        public virtual void setSpeed(int speed){
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
        }

        public int getGoalPosition()
        {
            return this.goalPosition;
        }

        public int getID()
        {
            return this.id;
        }

        public bool isInGoalPosition()
        {
            int margin = 20;
            if (this.id == 250) {
                margin = 20;
            }
            if (Math.Abs(dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L) - this.goalPosition) < margin )
            {
                
                //    Console.WriteLine("H\tMotorID: " + this.id + " PresPos: " + dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_H) + " Goalpos: " + this.goalPosition);
                //Console.WriteLine("L\tMotorID: " + this.id + " PresPos: " + dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L) + " Goalpos: " + this.goalPosition);
                return true;
            }
          
            Console.WriteLine("\t Nincs pozíción: MotorID: " + this.id + " PresPos: " + dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_H) + " Goalpos: " + this.goalPosition);
            //Console.WriteLine("L\tMotorID: " + this.id + " PresPos: " + dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L) + " Goalpos: " + this.goalPosition);
            return false;
        }


        public int getPresentPositon() 
        {
            return dynamixel.dxl_read_word(this.id, P_PRESENT_POSITION_L);
        }
    }
}
