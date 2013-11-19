using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ROBOTIS;
using System.Threading;

namespace amoba
{
    /// <summary>
    /// 
    /// </summary>
    class DuplaMotor : Motor
    {
        int masikid;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="masikid"></param>
        public DuplaMotor(int id, int masikid) : base(id)
        {
            this.masikid = masikid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public override void Run(int pos)
        {
            this.goalPosition = pos;
            dynamixel.dxl_write_word(this.id, P_GOAL_POSITION_L, pos);
            Thread.Sleep(40);
            dynamixel.dxl_write_word(this.masikid, P_GOAL_POSITION_L, pos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed"></param>
        public override void setSpeed(int speed)
        {
            dynamixel.dxl_write_word(this.id, P_SPEED, speed);
            dynamixel.dxl_write_word(this.masikid, P_SPEED, speed);
        }
    }
}
