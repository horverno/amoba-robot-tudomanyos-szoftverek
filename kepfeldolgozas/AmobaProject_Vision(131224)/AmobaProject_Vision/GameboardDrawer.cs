using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace AmobaProject_Vision
{
    class GameboardDrawer
    {
        private int cellsX;
        private int cellsY;
        private int width;
        private int height;
        private int thickness;
        private int[,] table;

        public GameboardDrawer(int cellsX, int cellsY, int width, int height, int thickness)
        {
            this.cellsX = cellsX;
            this.cellsY = cellsY;
            this.width = width;
            this.height = height;
            this.thickness = thickness;
            table = new int[width, height];
            for (int i = 0; i < cellsX; i++)
            {
                for (int j = 0; j < cellsY; j++)
                {
                    table[i, j] = -1;
                }
            }
        }

        public void setO(int i, int j) 
        {
            if (i >= 0 && i < cellsX && j >= 0 && j < cellsY)
            {
                table[i, j] = 0;
                //Console.WriteLine("O  " + i.ToString() + "  " + j.ToString());
            }
        }

        public void setX(int i, int j)
        {
            if (i >= 0 && i < cellsX && j >= 0 && j < cellsY)
            {
                table[i, j] = 1;
                //Console.WriteLine("X  " + i.ToString() + "  " + j.ToString());
            }
        }

        public void setFree(int i, int j)
        {
            if (i >= 0 && i < cellsX && j >= 0 && j < cellsY)
            {
                table[i, j] = -1;
            }
        }

        public Image<Bgr, byte> getGameboard()
        {
            Image<Bgr, byte> rv = new Image<Bgr, byte>(width, height, new Bgr(255, 255, 255));
            rv.Draw(new System.Drawing.Rectangle(0, 0, width, height), new Bgr(0, 0, 0), thickness);
            int cellWidth = width / cellsX;
            int cellHeight = height / cellsY;
            for (int i = 1; i < cellsX; i++)
            {
                rv.Draw(new LineSegment2D(new Point(cellWidth * i, 0), new Point(cellWidth * i, height)), new Bgr(0, 0, 0), thickness);
            }
            for (int i = 1; i < cellsY; i++)
            {
                rv.Draw(new LineSegment2D(new Point(0, cellHeight * i), new Point(width, cellHeight * i)), new Bgr(0, 0, 0), thickness);
            }
            for (int i = 0; i < cellsX; i++)
            {
                for (int j = 0; j < cellsY; j++)
                {
                    if (table[i, j] == 0)
                    {
                        rv.Draw(new Ellipse(new PointF(cellWidth * i + cellWidth / 2, cellHeight * j + cellHeight / 2), new SizeF(cellHeight, cellWidth), 0), new Bgr(0, 0, 0), thickness);
                    }
                    else if (table[i, j] == 1)
                    {
                        rv.Draw(new LineSegment2D(new Point(cellWidth * i, cellHeight * j), new Point(cellWidth * i + cellWidth, cellHeight * j + cellHeight)), new Bgr(0, 0, 0), thickness);
                        rv.Draw(new LineSegment2D(new Point(cellWidth * i + cellWidth, cellHeight * j), new Point(cellWidth * i, cellHeight * j + cellHeight)), new Bgr(0, 0, 0), thickness);
                    }
                }
            }
            return rv;
        }
    }
}
