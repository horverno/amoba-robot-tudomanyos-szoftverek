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
        private int goalPosition;
        
        


        
        public Motor(int id)
        {
            this.id = id;
        }

        public virtual void ThreadRun(int pos)
        {
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
            this.goalPosition = pos;
        }

        public virtual void setSpeed(int speed){
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
        }

        public int getID()
        {
            return this.id;
        }

        public bool isInGoalPosition()
        {
            if (Math.Abs(dynamixel.dxl_read_byte(this.id, P_PRESENT_POSITION_L) - this.goalPosition) < 10 )
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
