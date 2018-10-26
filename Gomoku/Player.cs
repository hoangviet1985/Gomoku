using System;
using System.Collections.Generic;

namespace Gomoku
{
    /// <summary>
    /// a player in game
    /// </summary>
    class Player
    {
        public Tuple<int, int> LatestMarkLoc { get; set; }

        public Player()
        {
            LatestMarkLoc = new Tuple<int, int>(-1, -1);
        }

        public bool MarkOnGameBoard(Dictionary<int, int> gameBoard, int x, int y, int markSymbol)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }
            if (gameBoard.ContainsKey(x * HyperParam.boardSide + y))
            {
                return false;
            }
            if (x * HyperParam.boardSide + y > HyperParam.boardSide * HyperParam.boardSide - 1)
            {
                return false;
            }
            gameBoard[x * HyperParam.boardSide + y] = markSymbol;
            LatestMarkLoc = new Tuple<int, int>(x, y);
            return true;
        }
    }

}
