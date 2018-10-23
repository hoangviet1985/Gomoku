using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gomoku
{
    class Utilities
    {
        public static Tuple<int, int> getGameBoardXFromMousePosition(Point p)
        {
            var x = (int)p.X;
            var y = (int)p.Y;
            var test = x % HyperParam.cellBoardSide;
            x = test < HyperParam.cellBoardSide / 2 ? (x / HyperParam.cellBoardSide) * HyperParam.cellBoardSide : (x / HyperParam.cellBoardSide + 1) * HyperParam.cellBoardSide;
            test = y % HyperParam.cellBoardSide;
            y = test < HyperParam.cellBoardSide / 2 ? (y / HyperParam.cellBoardSide) * HyperParam.cellBoardSide : (y / HyperParam.cellBoardSide + 1) * HyperParam.cellBoardSide;

            return new Tuple<int, int>(x, y);
        }
    }
}
