//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ROBOTIS;

//namespace amoba
//{

//    public class Leker
//    {
//        static Motor[] motrok = new Motor[7];

//        public const int P_GOAL_POSITION_L = 30;
//        public const int P_GOAL_POSITION_H = 31;
//        public const int P_PRESENT_POSITION_L = 36;
//        public const int P_PRESENT_POSITION_H = 37;
//        public const int P_MOVING = 46;
//        public const int P_SPEED = 32;


//        // Defulat setting
//        public const int DEFAULT_PORTNUM = 3; // COM3
//        public const int DEFAULT_BAUDNUM = 1; // 1Mbps
//        protected int id;
//        protected int goalPosition;

//        static void Main()
//        {


//            if (dynamixel.dxl_initialize(DEFAULT_PORTNUM, DEFAULT_BAUDNUM) == 0)
//            {
//                Console.WriteLine("Failed to open USB2Dynamixel!");
//                Console.WriteLine("Press any key to terminate...");
//                Console.ReadKey(true);
//                return;
//            }
//            else
//            {
//                Console.WriteLine("Succeed to open USB2Dynamixel!");
//            }

//            Motor motor1 = new Motor(9);
//            Motor motor2 = new Motor(10);
//            Motor motor3 = new Motor(11);
//            Motor motor4 = new Motor(12);
//            Motor motor5 = new Motor(250);

//            DuplaMotor motor6 = new DuplaMotor(13, 14);
//            DuplaMotor motor7 = new DuplaMotor(2, 16);

//            motrok[0] = motor1;
//            motrok[1] = motor2;
//            motrok[2] = motor3;
//            motrok[3] = motor4;
//            motrok[4] = motor5;
//            motrok[5] = motor6;
//            motrok[6] = motor7;


//            //System.Diagnostics.Debug.Write("ID :" + motrok[i].getID() + " - " + motrok[i].getPresentPositon() + "\n");

//            String temp = Console.ReadLine();

//            System.Diagnostics.Debug.Write("int[,] " + temp + "mov = new int[2, 7];\n" +
//                    temp + "mov[0, 0] = 2;\n" +
//                    temp + "mov[1, 0] = " + motrok[6].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 1] = 9;\n" +
//                    temp + "mov[1, 1] = " + motrok[0].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 2] = 10;\n" +
//                    temp + "mov[1, 2] = " + motrok[1].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 3] = 11;\n" +
//                    temp + "mov[1, 3] = " + motrok[2].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 4] = 12;\n" +
//                    temp + "mov[1, 4] = " + motrok[3].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 5] = 13;\n" +
//                    temp + "mov[1, 5] = " + motrok[5].getPresentPositon() + ";\n" +
//                    temp + "mov[0, 6] = 250;\n" +
//                    temp + "mov[1, 6] = " + motrok[4].getPresentPositon() + ";\n" +
//                    "Mozgas " + temp + " = new Mozgas(\"" + temp.ToUpper() + "\");\n" +
//                    temp + ".setTomb(" + temp + "mov);");



//            dynamixel.dxl_terminate();
//        }
//    }

//}