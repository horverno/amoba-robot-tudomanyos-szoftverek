using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using InterfaceModule;

namespace AmobaProject_Vision_Processor
{

    class GameboardDrawer
    {
        private int cellsX; //Oszlopok száma.
        private int cellsY; //Sorok száma.
        private int width; //Visszaadott kép szélessége.
        private int height; //Visszaadott kép magassága.
        private int thickness; //Vonal vastagság.
        private List<Piece>[,] table; //Táblának a tartalma.
        private int sensitivity; //Cellák mélysége.


        public GameboardDrawer(int cellsX, int cellsY, int width, int height, int thickness, int sensitivity) //Konstruktor.
        {
            this.sensitivity = sensitivity;
            this.cellsX = cellsX;
            this.cellsY = cellsY;
            this.width = width;
            this.height = height;
            this.thickness = thickness;
            table = new List<Piece>[cellsX, cellsY];
            for (int i = 0; i < cellsX; i++) //Táblázat kinullázása.
            {
                for (int j = 0; j < cellsY; j++)
                {
                    table[i, j] = new List<Piece>();
                    table[i, j].Add(Piece._Empty);
                }
            }
        }

        public void setPieces(int i, int j, Piece p) //Új bábu felvétele
        {
            if (table[i, j].Count == sensitivity) //Csak az utolsó sensitivity mennyiségű bábut tároljuk.
            {
                table[i, j].RemoveAt(0);
            }
            table[i, j].Add(p);
        }

        public Image<Bgr, byte> getGameboard() //Kirajzolja a táblát.
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
                    if (getPiece(i, j) == Piece.O)
                    {
                        rv.Draw(new Ellipse(new PointF(cellWidth * i + cellWidth / 2, cellHeight * j + cellHeight / 2), new SizeF(cellHeight*0.8f, cellWidth*0.8f), 0), new Bgr(0, 127, 0), thickness);
                    }
                    else if (getPiece(i, j) == Piece.X)
                    {
                        int w = (int)(cellWidth * 0.1);
                        int h = (int)(cellHeight * 0.1); 
                        rv.Draw(new LineSegment2D(new Point(cellWidth * i + w, cellHeight * j + h), new Point(cellWidth * i + cellWidth - w, cellHeight * j + cellHeight - h)), new Bgr(255, 0, 0), thickness);
                        rv.Draw(new LineSegment2D(new Point(cellWidth * i + cellWidth - w, cellHeight * j + h), new Point(cellWidth * i + w, cellHeight * j + cellHeight - h)), new Bgr(255, 0, 0), thickness);
                    }
                    else if (getPiece(i, j) == Piece._MoreThan1)
                    {
                        Font font = new Font("Arial", 80, FontStyle.Bold);
                        Graphics g = Graphics.FromImage(rv.Bitmap);
                        g.DrawString("?", font, Brushes.Red, new Point(cellWidth * i, cellHeight * j));
                        
                    }
                    else if (getPiece(i, j) == Piece._OutOfField)
                    {
                        Font font = new Font("Arial", 80, FontStyle.Bold);
                        Graphics g = Graphics.FromImage(rv.Bitmap);
                        g.DrawString("!", font, Brushes.Red, new Point(cellWidth * i, cellHeight * j));
                    }
                }
            }
            return rv;
        }

        public static bool Compare(GameboardDrawer gbd1, GameboardDrawer gbd2) //Két tábla tartalmát összehasonlítja
        {
            if (gbd1.cellsX != gbd2.cellsX)
            {
                return false;
            }
            if (gbd1.cellsY != gbd2.cellsY)
            {
                return false;
            }
            for (int i = 0; i < gbd1.cellsX; i++)
            {
                for (int j = 0; j < gbd1.cellsY; j++)
                {
                    if(gbd1.getPiece(i, j) != gbd2.getPiece(i, j))
                    {
                        return false;
                    }   
                }
            }
            return true;
        }

        public GameboardDrawer Copy() //Másolatot készít a tábláról.
        {
            GameboardDrawer rv = new GameboardDrawer(this.cellsX, this.cellsY, this.width, this.height, this.thickness, this.sensitivity);
            for (int i = 0; i < cellsX; i++)
            {
                for (int j = 0; j < cellsY; j++)
                {
                    rv.table[i, j] = new List<Piece>(this.table[i, j]);
                }
            }
            return rv;
        }

        public Piece[,] GetTable() //Visszaadja a tábla tartalmát
        {
            Piece[,] rv = new Piece[cellsX, cellsY];
            for (int i = 0; i < cellsX; i++)
            {
                for (int j = 0; j < cellsY; j++)
                {
                    rv[i, j] = getPiece(i, j);
                }
            }
            return rv;
        }

        private Piece getPiece(int x, int y) //Kiértékeljük az adott mező tartalmát
        {
            int[] tmp = new int[5];
            int max = 0;
            for (int i = 0; i < 5; i++)
            {
                tmp[i] = 0;
            }
            foreach (Piece item in table[x, y])
            {
                tmp[(int)item]++;
            }
            for (int i = 1; i < 5; i++)
            {
                if (tmp[i]>tmp[max])
                {
                    max = i;
                }
            }

            return (Piece)max;
        }

    }
}

