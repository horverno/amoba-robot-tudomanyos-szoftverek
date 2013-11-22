using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ROBOTIS;

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


            int[,] babumegymov = new int[2, 7];
            babumegymov[0, 0] = 2;
            babumegymov[1, 0] = 801;
            babumegymov[0, 1] = 9;
            babumegymov[1, 1] = 854;
            babumegymov[0, 2] = 10;
            babumegymov[1, 2] = 811;
            babumegymov[0, 3] = 11;
            babumegymov[1, 3] = 233;
            babumegymov[0, 4] = 12;
            babumegymov[1, 4] = 638;
            babumegymov[0, 5] = 13;
            babumegymov[1, 5] = 636;
            babumegymov[0, 6] = 250;
            babumegymov[1, 6] = 866;
            Mozgas babumegy = new Mozgas("BABUMEGY");
            babumegy.setTomb(babumegymov);


            int[,] babu_felmov = new int[2, 7];
            babu_felmov[0, 0] = 2;
            babu_felmov[1, 0] = 801;
            babu_felmov[0, 1] = 9;
            babu_felmov[1, 1] = 854;
            babu_felmov[0, 2] = 10;
            babu_felmov[1, 2] = 811;
            babu_felmov[0, 3] = 11;
            babu_felmov[1, 3] = 233;
            babu_felmov[0, 4] = 12;
            babu_felmov[1, 4] = 638;
            babu_felmov[0, 5] = 13;
            babu_felmov[1, 5] = 690;
            babu_felmov[0, 6] = 250;
            babu_felmov[1, 6] = 813;
            Mozgas babu_fel = new Mozgas("BABUFEL");
            babu_fel.setTomb(babu_felmov);

            int[,] osszefogmov = new int[2, 7];
            osszefogmov[0, 0] = 2;
            osszefogmov[1, 0] = 801;
            osszefogmov[0, 1] = 9;
            osszefogmov[1, 1] = 854;
            osszefogmov[0, 2] = 10;
            osszefogmov[1, 2] = 811;
            osszefogmov[0, 3] = 11;
            osszefogmov[1, 3] = 233;
            osszefogmov[0, 4] = 12;
            osszefogmov[1, 4] = 638;
            osszefogmov[0, 5] = 13;
            osszefogmov[1, 5] = 636;
            osszefogmov[0, 6] = 250;
            osszefogmov[1, 6] = 813;
            Mozgas osszefog = new Mozgas("OSSZEFOG");
            osszefog.setTomb(osszefogmov);

            int[,] elengedmov = new int[2, 7];
            elengedmov[0, 0] = 2;
            elengedmov[1, 0] = 781;
            elengedmov[0, 1] = 9;
            elengedmov[1, 1] = 761;
            elengedmov[0, 2] = 10;
            elengedmov[1, 2] = 250;
            elengedmov[0, 3] = 11;
            elengedmov[1, 3] = 316;
            elengedmov[0, 4] = 12;
            elengedmov[1, 4] = 686;
            elengedmov[0, 5] = 13;
            elengedmov[1, 5] = 666;
            elengedmov[0, 6] = 250;
            elengedmov[1, 6] = 833;
            Mozgas elenged = new Mozgas("ELENGED");
            elenged.setTomb(elengedmov);

            int[,] konyokBemov = new int[2, 7];
            konyokBemov[0, 0] = 2;
            konyokBemov[1, 0] = 800;
            konyokBemov[0, 1] = 9;
            konyokBemov[1, 1] = 806;
            konyokBemov[0, 2] = 10;
            konyokBemov[1, 2] = 630;
            konyokBemov[0, 3] = 11;
            konyokBemov[1, 3] = 518;
            konyokBemov[0, 4] = 12;
            konyokBemov[1, 4] = 803;
            konyokBemov[0, 5] = 13;
            konyokBemov[1, 5] = 486;
            konyokBemov[0, 6] = 250;
            konyokBemov[1, 6] = 818;
            Mozgas konyokBe = new Mozgas("KONYOKBE");
            konyokBe.setTomb(konyokBemov);


            int[,] poz00mov = new int[2, 7];
            poz00mov[0, 0] = 2;
            poz00mov[1, 0] = 818;
            poz00mov[0, 1] = 9;
            poz00mov[1, 1] = 837;
            poz00mov[0, 2] = 10;
            poz00mov[1, 2] = 630;
            poz00mov[0, 3] = 11;
            poz00mov[1, 3] = 337;
            poz00mov[0, 4] = 12;
            poz00mov[1, 4] = 701;
            poz00mov[0, 5] = 13;
            poz00mov[1, 5] = 656;
            poz00mov[0, 6] = 250;
            poz00mov[1, 6] = 813;
            Mozgas poz00 = new Mozgas("POZ00");
            poz00.setTomb(poz00mov);

            int[,] poz00felmov = new int[2, 7];
            poz00felmov[0, 0] = 2;
            poz00felmov[1, 0] = 819;
            poz00felmov[0, 1] = 9;
            poz00felmov[1, 1] = 837;
            poz00felmov[0, 2] = 10;
            poz00felmov[1, 2] = 631;
            poz00felmov[0, 3] = 11;
            poz00felmov[1, 3] = 337;
            poz00felmov[0, 4] = 12;
            poz00felmov[1, 4] = 702;
            poz00felmov[0, 5] = 13;
            poz00felmov[1, 5] = 708;
            poz00felmov[0, 6] = 250;
            poz00felmov[1, 6] = 813;
            Mozgas poz00fel = new Mozgas("POZ00FEL");
            poz00fel.setTomb(poz00felmov);

            int[,] poz00leteszmov = new int[2, 7];
            poz00leteszmov[0, 0] = 2;
            poz00leteszmov[1, 0] = 818;
            poz00leteszmov[0, 1] = 9;
            poz00leteszmov[1, 1] = 837;
            poz00leteszmov[0, 2] = 10;
            poz00leteszmov[1, 2] = 630;
            poz00leteszmov[0, 3] = 11;
            poz00leteszmov[1, 3] = 337;
            poz00leteszmov[0, 4] = 12;
            poz00leteszmov[1, 4] = 701;
            poz00leteszmov[0, 5] = 13;
            poz00leteszmov[1, 5] = 656;
            poz00leteszmov[0, 6] = 250;
            poz00leteszmov[1, 6] = 850;
            Mozgas poz00letesz = new Mozgas("POZ00LETESZ");
            poz00letesz.setTomb(poz00leteszmov);


            int[,] poz10mov = new int[2, 7];
            poz10mov[0, 0] = 2;
            poz10mov[1, 0] = 805;
            poz10mov[0, 1] = 9;
            poz10mov[1, 1] = 829;
            poz10mov[0, 2] = 10;
            poz10mov[1, 2] = 676;
            poz10mov[0, 3] = 11;
            poz10mov[1, 3] = 359;
            poz10mov[0, 4] = 12;
            poz10mov[1, 4] = 732;
            poz10mov[0, 5] = 13;
            poz10mov[1, 5] = 658;
            poz10mov[0, 6] = 250;
            poz10mov[1, 6] = 813;
            Mozgas poz10 = new Mozgas("POZ10");
            poz10.setTomb(poz10mov);

            int[,] poz10felmov = new int[2, 7];
            poz10felmov[0, 0] = 2;
            poz10felmov[1, 0] = 806;
            poz10felmov[0, 1] = 9;
            poz10felmov[1, 1] = 829;
            poz10felmov[0, 2] = 10;
            poz10felmov[1, 2] = 677;
            poz10felmov[0, 3] = 11;
            poz10felmov[1, 3] = 360;
            poz10felmov[0, 4] = 12;
            poz10felmov[1, 4] = 730;
            poz10felmov[0, 5] = 13;
            poz10felmov[1, 5] = 708;
            poz10felmov[0, 6] = 250;
            poz10felmov[1, 6] = 813;
            Mozgas poz10fel = new Mozgas("POZ10FEL");  //kicsit pontosítani kell
            poz10fel.setTomb(poz10felmov);

            int[,] poz10leteszmov = new int[2, 7];
            poz10leteszmov[0, 0] = 2;
            poz10leteszmov[1, 0] = 806;
            poz10leteszmov[0, 1] = 9;
            poz10leteszmov[1, 1] = 829;
            poz10leteszmov[0, 2] = 10;
            poz10leteszmov[1, 2] = 677;
            poz10leteszmov[0, 3] = 11;
            poz10leteszmov[1, 3] = 360;
            poz10leteszmov[0, 4] = 12;
            poz10leteszmov[1, 4] = 730;
            poz10leteszmov[0, 5] = 13;
            poz10leteszmov[1, 5] = 708;
            poz10leteszmov[0, 6] = 250;
            poz10leteszmov[1, 6] = 850;
            Mozgas poz10letesz = new Mozgas("POZ10LETESZ");
            poz10letesz.setTomb(poz10leteszmov);



            int[,] poz20mov = new int[2, 7];
            poz20mov[0, 0] = 2;
            poz20mov[1, 0] = 805;
            poz20mov[0, 1] = 9;
            poz20mov[1, 1] = 829;
            poz20mov[0, 2] = 10;
            poz20mov[1, 2] = 714;
            poz20mov[0, 3] = 11;
            poz20mov[1, 3] = 470;
            poz20mov[0, 4] = 12;
            poz20mov[1, 4] = 717;
            poz20mov[0, 5] = 13;
            poz20mov[1, 5] = 665;
            poz20mov[0, 6] = 250;
            poz20mov[1, 6] = 813;
            Mozgas poz20 = new Mozgas("POZ20");
            poz20.setTomb(poz20mov);

            int[,] poz20felmov = new int[2, 7];
            poz20felmov[0, 0] = 2;
            poz20felmov[1, 0] = 806;
            poz20felmov[0, 1] = 9;
            poz20felmov[1, 1] = 829;
            poz20felmov[0, 2] = 10;
            poz20felmov[1, 2] = 714;
            poz20felmov[0, 3] = 11;
            poz20felmov[1, 3] = 471;
            poz20felmov[0, 4] = 12;
            poz20felmov[1, 4] = 716;
            poz20felmov[0, 5] = 13;
            poz20felmov[1, 5] = 737;
            poz20felmov[0, 6] = 250;
            poz20felmov[1, 6] = 813;
            Mozgas poz20fel = new Mozgas("POZ20FEL");           // A pozíció nioncs a cél felett, amúgy kiráély
            poz20fel.setTomb(poz20felmov);

            int[,] poz20leteszmov = new int[2, 7];
            poz20leteszmov[0, 0] = 2;
            poz20leteszmov[1, 0] = 806;
            poz20leteszmov[0, 1] = 9;
            poz20leteszmov[1, 1] = 829;
            poz20leteszmov[0, 2] = 10;
            poz20leteszmov[1, 2] = 714;
            poz20leteszmov[0, 3] = 11;
            poz20leteszmov[1, 3] = 471;
            poz20leteszmov[0, 4] = 12;
            poz20leteszmov[1, 4] = 716;
            poz20leteszmov[0, 5] = 13;
            poz20leteszmov[1, 5] = 737;
            poz20leteszmov[0, 6] = 250;
            poz20leteszmov[1, 6] = 850;
            Mozgas poz20letesz = new Mozgas("POZ20LETESZ");
            poz20letesz.setTomb(poz20leteszmov);

            int[,] poz30mov = new int[2, 7];
            poz30mov[0, 0] = 2;
            poz30mov[1, 0] = 805;
            poz30mov[0, 1] = 9;
            poz30mov[1, 1] = 869;
            poz30mov[0, 2] = 10;
            poz30mov[1, 2] = 713;
            poz30mov[0, 3] = 11;
            poz30mov[1, 3] = 580;
            poz30mov[0, 4] = 12;
            poz30mov[1, 4] = 667;
            poz30mov[0, 5] = 13;
            poz30mov[1, 5] = 672;
            poz30mov[0, 6] = 250;
            poz30mov[1, 6] = 813;
            Mozgas poz30 = new Mozgas("POZ30");
            poz30.setTomb(poz30mov);

            int[,] poz30felmov = new int[2, 7];
            poz30felmov[0, 0] = 2;
            poz30felmov[1, 0] = 805;
            poz30felmov[0, 1] = 9;
            poz30felmov[1, 1] = 869;
            poz30felmov[0, 2] = 10;
            poz30felmov[1, 2] = 714;
            poz30felmov[0, 3] = 11;
            poz30felmov[1, 3] = 580;
            poz30felmov[0, 4] = 12;
            poz30felmov[1, 4] = 667;
            poz30felmov[0, 5] = 13;
            poz30felmov[1, 5] = 719;
            poz30felmov[0, 6] = 250;
            poz30felmov[1, 6] = 813;
            Mozgas poz30fel = new Mozgas("POZ30FEL");
            poz30fel.setTomb(poz30felmov);

            int[,] poz30leteszmov = new int[2, 7];
            poz30leteszmov[0, 0] = 2;
            poz30leteszmov[1, 0] = 805;
            poz30leteszmov[0, 1] = 9;
            poz30leteszmov[1, 1] = 869;
            poz30leteszmov[0, 2] = 10;
            poz30leteszmov[1, 2] = 713;
            poz30leteszmov[0, 3] = 11;
            poz30leteszmov[1, 3] = 580;
            poz30leteszmov[0, 4] = 12;
            poz30leteszmov[1, 4] = 667;
            poz30leteszmov[0, 5] = 13;
            poz30leteszmov[1, 5] = 672;
            poz30leteszmov[0, 6] = 250;
            poz30leteszmov[1, 6] = 850;
            Mozgas poz30letesz = new Mozgas("POZ30LETESZ");
            poz30letesz.setTomb(poz30leteszmov);

            int[,] poz30felnyitmov = new int[2, 7];
            poz30felnyitmov[0, 0] = 2;
            poz30felnyitmov[1, 0] = 805;
            poz30felnyitmov[0, 1] = 9;
            poz30felnyitmov[1, 1] = 869;
            poz30felnyitmov[0, 2] = 10;
            poz30felnyitmov[1, 2] = 714;
            poz30felnyitmov[0, 3] = 11;
            poz30felnyitmov[1, 3] = 580;
            poz30felnyitmov[0, 4] = 12;
            poz30felnyitmov[1, 4] = 667;
            poz30felnyitmov[0, 5] = 13;
            poz30felnyitmov[1, 5] = 719;
            poz30felnyitmov[0, 6] = 250;
            poz30felnyitmov[1, 6] = 850;
            Mozgas poz30felnyit = new Mozgas("POZ30FELNYIT");
            poz30felnyit.setTomb(poz30felnyitmov);


            //pihenőből bábúért

            int[,] _veddlemov = new int[2, 7];
            _veddlemov[0, 0] = 2;
            _veddlemov[1, 0] = 800;
            _veddlemov[0, 1] = 9;
            _veddlemov[1, 1] = 828;
            _veddlemov[0, 2] = 10;
            _veddlemov[1, 2] = 428;
            _veddlemov[0, 3] = 11;
            _veddlemov[1, 3] = 442;
            _veddlemov[0, 4] = 12;
            _veddlemov[1, 4] = 834;
            _veddlemov[0, 5] = 13;
            _veddlemov[1, 5] = 831;
            _veddlemov[0, 6] = 250;
            _veddlemov[1, 6] = 813;
            Mozgas _veddle = new Mozgas("_VEDDLE");
            _veddle.setTomb(_veddlemov);

            /*NULL pozicíó begin*/
            int[,] npmov = new int[2, 7];
            npmov[0, 0] = 2;
            npmov[1, 0] = 800;
            npmov[0, 1] = 9;
            npmov[1, 1] = 806;
            npmov[0, 2] = 10;
            npmov[1, 2] = 630;
            npmov[0, 3] = 11;
            npmov[1, 3] = 518;
            npmov[0, 4] = 12;
            npmov[1, 4] = 532;
            npmov[0, 5] = 13;
            npmov[1, 5] = 486;
            //npmov[0,6]=14;
            //npmov[1,6]=325;
            //npmov[0,7]=16;
            //npmov[1,7]=826;
            npmov[0, 6] = 250;
            npmov[1, 6] = 818; //643

            Mozgas np = new Mozgas("NP");
            np.setTomb(npmov);
            /*NULL pozicíó end*/



            /*bábuért megy begin*/
            int[,] bmmov = new int[2, 7];
            bmmov[0, 0] = 2;
            bmmov[1, 0] = 800;
            bmmov[0, 1] = 9;
            bmmov[1, 1] = 190;
            bmmov[0, 2] = 10;
            bmmov[1, 2] = 525;
            bmmov[0, 3] = 11;
            bmmov[1, 3] = 180;
            bmmov[0, 4] = 12;
            bmmov[1, 4] = 689;
            bmmov[0, 5] = 13;
            bmmov[1, 5] = 312;
            //bmmov[0,6]=14;
            //bmmov[1,6]=325;
            //bmmov[0,7]=16;
            //bmmov[1,7]=826;
            bmmov[0, 6] = 250;
            bmmov[1, 6] = 900;

            Mozgas bm = new Mozgas("BM");
            bm.setTomb(bmmov);
            /*bábuért megy end*/

            /*bábut markol begin*/
            int[,] bmarkolmov = new int[2, 7];
            bmarkolmov[0, 0] = 2;
            bmarkolmov[1, 0] = 782;
            bmarkolmov[0, 1] = 9;
            bmarkolmov[1, 1] = 217;
            bmarkolmov[0, 2] = 10;
            bmarkolmov[1, 2] = 525;
            bmarkolmov[0, 3] = 11;
            bmarkolmov[1, 3] = 221;
            bmarkolmov[0, 4] = 12;
            bmarkolmov[1, 4] = 616;
            bmarkolmov[0, 5] = 13;
            bmarkolmov[1, 5] = 386;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bmarkolmov[0, 6] = 250;
            bmarkolmov[1, 6] = 900;

            Mozgas bmarkol = new Mozgas("BMM");
            bmarkol.setTomb(bmarkolmov);

            /*bábut markol end*/

            /*bábut markol2 (elmozdítja a felvettet) begin*/
            int[,] bmarkolmovv = new int[2, 7];
            bmarkolmovv[0, 0] = 2;
            bmarkolmovv[1, 0] = 800;
            bmarkolmovv[0, 1] = 9;
            bmarkolmovv[1, 1] = 190;
            bmarkolmovv[0, 2] = 10;
            bmarkolmovv[1, 2] = 525;
            bmarkolmovv[0, 3] = 11;
            bmarkolmovv[1, 3] = 180;
            bmarkolmovv[0, 4] = 12;
            bmarkolmovv[1, 4] = 689;
            bmarkolmovv[0, 5] = 13;
            bmarkolmovv[1, 5] = 312;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bmarkolmovv[0, 6] = 250;
            bmarkolmovv[1, 6] = 818;

            Mozgas bmv = new Mozgas("BMV");
            bmv.setTomb(bmarkolmovv);


            // markolás -> összeszorít
            /*bábut összeszorít begin*/

            int[,] bszorit = new int[2, 7];

            bszorit[0, 0] = 2;
            bszorit[1, 0] = 782;
            bszorit[0, 1] = 9;
            bszorit[1, 1] = 217;
            bszorit[0, 2] = 10;
            bszorit[1, 2] = 525;
            bszorit[0, 3] = 11;
            bszorit[1, 3] = 221;
            bszorit[0, 4] = 12;
            bszorit[1, 4] = 616;
            bszorit[0, 5] = 13;
            bszorit[1, 5] = 386;
            //bmarkolmov[0, 6] = 14;
            //bmarkolmov[1, 6] = 325;
            //bmarkolmov[0, 7] = 16;
            //bmarkolmov[1, 7] = 826;
            bszorit[0, 6] = 250;
            bszorit[1, 6] = 818;

            Mozgas bmszorit = new Mozgas("BMSZ");
            bmszorit.setTomb(bszorit);

            /*bábut összeszorít end*/









            /* mezők begin */


            int[,] a1mov = new int[2, 3];
            a1mov[0, 0] = 2;
            a1mov[1, 0] = 826;
            a1mov[0, 1] = 9;
            a1mov[1, 1] = 181;
            a1mov[0, 2] = 10;
            a1mov[1, 2] = 558;

            Mozgas a1 = new Mozgas("A1");
            a1.setTomb(a1mov);

            int[,] a2mov = new int[2, 3];
            a2mov[0, 0] = 250;
            a2mov[1, 0] = 536;
            a2mov[0, 1] = 10;
            a2mov[1, 1] = 622;
            a2mov[0, 2] = 12;
            a2mov[1, 2] = 518;

            Mozgas a2 = new Mozgas("A2");
            a2.setTomb(a2mov);

            /*mezők end*/

            /* mozgások aggregálása egy tömbbe */

            mozg[0] = np;

            mozg[1] = _veddle;
            mozg[2] = babumegy;
            mozg[3] = babu_fel;
            mozg[4] = poz30;
            mozg[5] = poz30fel;
            mozg[6] = poz30letesz;
            mozg[7] = konyokBe;
            mozg[8] = osszefog;
            mozg[9] = elenged;
            mozg[10] = poz30felnyit;





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
                                            if (mozgatas("POZ30FEL"))
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
            for (int i = 0; i < mozg.Count(); i++)// mozg-okon megy végig
            {
                if (mozg[i].getCel().Equals(celmezo))   // azt a mozgást keresem ami a bemeneti paraméternek megfelel
                {
                    int[,] akttomb = mozg[i].getTomb();
                    Console.WriteLine(motorok.Count());
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

    }
}
