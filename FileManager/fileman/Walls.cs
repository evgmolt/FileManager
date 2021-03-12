using System;
using System.Collections.Generic;

namespace fileman
{
    class Walls
    {
        char horizontalBorder = '\u2500';
        char verticalBorder = '\u2502';
        char downAndRightBorder = '\u2518';
        char downAndLeftBroder = '\u2514';
        char upAndLeftBroder = '\u250C';
        char upAndRightBroder = '\u2510';
        char tLeft = '\u251C';
        char tRight = '\u2524';

        public int messPos;
        List<Figure> wallsList;
        List<Point> anglesList;
        public Walls(int mapWidth, int mapHeight)
        {
            int mapMessHeight = 3;
            mapHeight = mapHeight - 2;
            messPos = mapHeight - mapMessHeight;
            wallsList = new List<Figure>();
            anglesList = new List<Point>();
            HorizontalLine upLine = new HorizontalLine(0, mapWidth, 0, horizontalBorder);
            HorizontalLine downLine = new HorizontalLine(0, mapWidth, mapHeight, horizontalBorder);
            VerticalLine leftLine = new VerticalLine(0, 0, mapHeight, verticalBorder);
            VerticalLine rightLine = new VerticalLine(mapWidth, 0, mapHeight , verticalBorder);
            wallsList.Add(upLine);
            wallsList.Add(downLine);
            wallsList.Add(leftLine);
            wallsList.Add(rightLine);
            Point downRight = new Point(mapWidth , mapHeight , downAndRightBorder);
            anglesList.Add(downRight);
            Point downLeft = new Point(0, mapHeight , downAndLeftBroder);
            anglesList.Add(downLeft);
            Point upLeft = new Point(0, 0, upAndLeftBroder);
            anglesList.Add(upLeft);
            Point upRight = new Point(mapWidth , 0, upAndRightBroder);
            anglesList.Add(upRight);
            HorizontalLine messLine = new HorizontalLine(0, mapWidth , messPos, horizontalBorder);
            wallsList.Add(messLine);
            Point ptLeft = new Point(0, messPos, tLeft);
            anglesList.Add(ptLeft);
            Point ptRight = new Point(mapWidth , messPos, tRight);
            anglesList.Add(ptRight);
        }

        internal void Draw()
        {
            Console.BackgroundColor = Const.backColor;
            Console.Clear();
            foreach(var wall in wallsList)
            {
                wall.Draw();
            }
            foreach (var ang in anglesList)
            {
                ang.Draw();
            }
        }

        internal bool IsHit(Figure figure)
        {
            foreach(var wall in wallsList)
            {
                if(wall.IsHit(figure))
                {
                    return true;
                }
            }
            return false;
        }
    }
}