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

        List<Figure> wallsList;
        List<Point> anglesList;
        public Walls(int mapWidth, int mapHeight)
        {
            wallsList = new List<Figure>();
            anglesList = new List<Point>();
            HorizontalLine upLine = new HorizontalLine(0, mapWidth - 2, 0, horizontalBorder);
            HorizontalLine downLine = new HorizontalLine(0, mapWidth-2, mapHeight - 2, horizontalBorder);
            VerticalLine leftLine = new VerticalLine(0, 0, mapHeight - 2, verticalBorder);
            VerticalLine rightLine = new VerticalLine(mapWidth - 2, 0, mapHeight - 2, verticalBorder);
            wallsList.Add(upLine);
            wallsList.Add(downLine);
            wallsList.Add(leftLine);
            wallsList.Add(rightLine);
            Point downRight = new Point(mapWidth - 2, mapHeight - 2, downAndRightBorder);
            anglesList.Add(downRight);
            Point downLeft = new Point(0, mapHeight - 2, downAndLeftBroder);
            anglesList.Add(downLeft);
            Point upLeft = new Point(0, 0, upAndLeftBroder);
            anglesList.Add(upLeft);
            Point upRight = new Point(mapWidth - 2, 0, upAndRightBroder);
            anglesList.Add(upRight);
        }

        internal void Draw()
        {
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