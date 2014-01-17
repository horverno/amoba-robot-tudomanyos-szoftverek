using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AmobaProject_Vision_OCR
{
    //Felismert mintákat tárolja el azok lényeges tulajdonságaival.
    [Serializable]
    public class Template
    {
        public string name;
        public Contour contour;
        public Contour autoCorr;
        public Point startPoint;
        public bool preferredAngleNoMore90 = false;

        public int autoCorrDescriptor1;
        public int autoCorrDescriptor2;
        public int autoCorrDescriptor3;
        public int autoCorrDescriptor4;
        public double contourNorma;
        public double sourceArea;
        [NonSerialized]
        public object tag;

        //A kapott pontok alapján képzi a kontúrokat és igazítja a méretét a sablon méretéhez.
        public Template(Point[] points, double sourceArea, int templateSize)
        {
            this.sourceArea = sourceArea;
            startPoint = points[0];
            contour = new Contour(points);
            contour.Equalization(templateSize);
            contourNorma = contour.Norma;
            autoCorr = contour.AutoCorrelation(true);

            CalcAutoCorrDescriptions();
        }


        static int[] filter1 = { 1, 1, 1, 1 };
        static int[] filter2 = { -1, -1, 1, 1 };
        static int[] filter3 = { -1, 1, 1, -1 };
        static int[] filter4 = { -1, 1, -1, 1 };

        //Korrelációs tényezőket számítja ki. Az összehasonlításhoz szükséges.
        public void CalcAutoCorrDescriptions()
        {
            int count = autoCorr.Count;
            double sum1 = 0;
            double sum2 = 0;
            double sum3 = 0;
            double sum4 = 0;
            for (int i = 0; i < count; i++)
            {
                double v = autoCorr[i].Norma;
                int j = 4 * i / count;

                sum1 += filter1[j] * v;
                sum2 += filter2[j] * v;
                sum3 += filter3[j] * v;
                sum4 += filter4[j] * v;
            }

            autoCorrDescriptor1 = (int)(100 * sum1 / count);
            autoCorrDescriptor2 = (int)(100 * sum2 / count);
            autoCorrDescriptor3 = (int)(100 * sum3 / count);
            autoCorrDescriptor4 = (int)(100 * sum4 / count);
        }

    }

    [Serializable]
    public class Templates : List<Template>
    {
        public int templateSize = 30;
    }
}
