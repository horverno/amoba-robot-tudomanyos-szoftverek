using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;

namespace AmobaProject_Vision_OCR
{

    //Sablonok generálására szolgál.
    public static class TemplateGenerator
    {
        //A kapott CharacterDetector-hoz elkészíti a kapott karakterekhez tartozó sablonokat a megadott betütípusban. Ez lesz a felismerés alapja.
        public static void GenerateChars(CharacterDetector processor, char[] chars, Font font)
        {
            Bitmap bmp = new Bitmap(400, 400);
            font = new Font(font.FontFamily, 140, font.Style);
            Graphics gr = Graphics.FromImage(bmp);
            //
            processor.onlyFindContours = true;
            foreach (char c in chars)
            {
                gr.Clear(Color.White);
                gr.DrawString(c.ToString(), font, Brushes.Black, 5, 5);
                GenerateTemplate(processor, bmp, c.ToString());
            }
            processor.onlyFindContours = false;
        }

        //Felismeri a most már képi formában megadott karaktert és eltárolja a sablonok között.
        private static void GenerateTemplate(CharacterDetector processor, Bitmap bmp, string name)
        {
            List<int[]> tmp = new List<int[]>();
            processor.ProcessImage(new Image<Bgr, byte>(bmp), out tmp);
            
            if (processor.samples.Count > 0)
            {
                processor.samples.Sort((t1, t2) => -t1.sourceArea.CompareTo(t2.sourceArea));
                processor.samples[0].name = name;
                processor.templates.Add(processor.samples[0]);
            }
        }
    }
}
