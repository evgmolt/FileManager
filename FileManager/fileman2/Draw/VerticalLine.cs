using System.Collections.Generic;

namespace fileman2
{
    class VerticalLine : Figure
    {
        public VerticalLine(int x, int yTop, int yBot, char sym)
        {
            pList = new List<Point>();
            for (int y = yTop; y <= yBot; y++)
            {
                Point p = new Point(x, y, sym);
                pList.Add(p);
            }
        }

    }
}
