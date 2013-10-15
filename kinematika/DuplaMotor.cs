using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROBOTIS;
using System.Threading;

namespace amoba
{
    class DuplaMotor : Motor
    {
        int masikid;
        public DuplaMotor(int id, int masikid) : base(id)
        {
            this.masikid = masikid;
        }

        public override void Run(int pos)
        {
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
            Thread.Sleep(40);
            dynamixel.dxl_write_word(this.masikid, P_GOAL_POSITION_L, pos);
        }

        public override void setSpeed(int speed)
        {
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
            dynamixel.dxl_write_word(this.masikid, P_SPEED, speed);
        }
    }
}
