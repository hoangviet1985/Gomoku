using System;
using System.Collections.Generic;
using System.Linq;

namespace Gomoku
{
    /// <summary>
    /// Description: This class represent a player in game
    /// Author: Viet Dinh, Jooseppi Luna, Iris Wang
    /// </summary>
    class Player
    {
        /// <summary>
        /// holds the latest location that the player marked on the gameboard
        /// </summary>
        public Tuple<int, int> LatestMarkLoc { get; set; }

        public Player()
        {
            LatestMarkLoc = new Tuple<int, int>(-1, -1);
        }

        /// <summary>
        /// Human player marks on the game board through mouse click
        /// </summary>
        /// <param name="gameBoard"> the dictionary represents the game board's status</param>
        /// <param name="x">x coordiante of the mark on the gameboard</param>
        /// <param name="y">x coordiante of the mark on the gameboard</param>
        /// <param name="markSymbol">number represents symbol of the player (1 or 2)</param>
        /// <param name="gameBoardMinX">min x of marks on game board</param>
        /// <param name="gameBoardMinY">min y of marks on game board</param>
        /// <param name="gameBoardMaxX">max x of marks on game board</param>
        /// <param name="gameBoardMaxY">max y of marks on game board</param>
        /// <returns>value indicates marking result</returns>
        public bool HumanMarksOnGameBoard(
            Dictionary<int, int> gameBoard,
            int x, 
            int y, 
            int markSymbol,
            ref int gameBoardMinX,
            ref int gameBoardMinY,
            ref int gameBoardMaxX,
            ref int gameBoardMaxY)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }
            if (gameBoard.ContainsKey(y * HyperParam.boardSide + x))
            {
                return false;
            }
            if (y * HyperParam.boardSide + x > HyperParam.boardSide * HyperParam.boardSide - 1)
            {
                return false;
            }
            gameBoard[y * HyperParam.boardSide + x] = markSymbol;
            LatestMarkLoc = new Tuple<int, int>(x, y);
            if(gameBoardMinX > x)
            {
                gameBoardMinX = x;
            }
            if(gameBoardMinY > y)
            {
                gameBoardMinY = y;
            }
            if(gameBoardMaxX < x)
            {
                gameBoardMaxX = x;
            }
            if(gameBoardMaxY < y)
            {
                gameBoardMaxY = y;
            }
            return true;
        }

        /// <summary>
        /// An AI player marks on the game board
        /// </summary>
        /// <param name="gameBoard">the dictionary represents the gameboard</param>
        /// <param name="x">x coordinate of the mark</param>
        /// <param name="y">x coordinate of the mark</param>
        /// <param name="markSymbol">mark symbol of the player (1 or 2)</param>
        /// <param name="gameBoardMinX">minimum x coordinate of existing marks</param>
        /// <param name="gameBoardMinY">minimum y coordinate of existing marks</param>
        /// <param name="gameBoardMaxX">maximum x coordinate of existing marks</param>
        /// <param name="gameBoardMaxY">maximum y coordinate of existing marks</param>
        public void AIMarksOnGameBoard(
            Dictionary<int, int> gameBoard, 
            int x, 
            int y, 
            int markSymbol,
            ref int gameBoardMinX,
            ref int gameBoardMinY,
            ref int gameBoardMaxX,
            ref int gameBoardMaxY)
        {
            gameBoard[y * HyperParam.boardSide + x] = markSymbol;
            LatestMarkLoc = new Tuple<int, int>(x, y);
            if (gameBoardMinX > x)
            {
                gameBoardMinX = x;
            }
            if (gameBoardMinY > y)
            {
                gameBoardMinY = y;
            }
            if (gameBoardMaxX < x)
            {
                gameBoardMaxX = x;
            }
            if (gameBoardMaxY < y)
            {
                gameBoardMaxY = y;
            }
        }

        #region Greedy Search AI

        /// <summary>
        /// simple AI agent computes location for its next mark
        /// </summary>
        /// <param name="gameBoard">the diction represents the gameboard</param>
        /// <param name="oponentLatestMark">location of last oponent's mark</param>
        /// <param name="myMarkSymbol">the player's symbol (1 or 2) as the dictionary contains only 
        /// 0 (non-marked location), 1, or 2 </param>
        /// <param name="opponentMarkSymbol">oponent symbol (1 or 2) as the dictionary contains only 
        /// 0 (non-marked location), 1, or 2 </param>
        /// <param name="gameBoardMinX">min x of marks on game board</param>
        /// <param name="gameBoardMinY">min y of marks on game board</param>
        /// <param name="gameBoardMaxX">max x of marks on game board</param>
        /// <param name="gameBoardMaxY">max y of marks on game board</param>
        /// <returns></returns>
        public Tuple<int, int> GreedySearchAIReasoning(
            Dictionary<int, int> gameBoard, 
            Tuple<int, int> opponentLatestMark, 
            int myMarkSymbol, 
            int opponentMarkSymbol,
            ref int gameBoardMinX,
            ref int gameBoardMinY,
            ref int gameBoardMaxX,
            ref int gameBoardMaxY)
        {
            if(gameBoard.Count == 0)
            {
                return new Tuple<int, int>(HyperParam.boardSide / 2, HyperParam.boardSide / 2);
            }

            Tuple<int, int> nextMark = CheckInstantWin(gameBoard, LatestMarkLoc, myMarkSymbol);
            if (nextMark != null)
            {
                return nextMark;
            }

            nextMark = CheckInstantWin(gameBoard, opponentLatestMark, opponentMarkSymbol);
            if (nextMark != null)
            {
                return nextMark;
            }

            nextMark = CheckSemiInstantWin(gameBoard, LatestMarkLoc, myMarkSymbol);
            if (nextMark != null)
            {
                return nextMark;
            }

            nextMark = CheckSemiInstantWin(gameBoard, opponentLatestMark, opponentMarkSymbol);
            if (nextMark != null)
            {
                return nextMark;
            }

            var nextMarkWithScore = GreedySearch(
                gameBoard,
                myMarkSymbol,
                opponentMarkSymbol,
                gameBoardMinX,
                gameBoardMinY,
                gameBoardMaxX,
                gameBoardMaxY);

            if (nextMarkWithScore != null)
                return new Tuple<int, int>(nextMarkWithScore.Item1, nextMarkWithScore.Item2);
            else
                return null;
        }

        /// <summary>
        /// Checks if a player can win instantly
        /// </summary>
        /// <param name="gameBoard">The dictionary represents the game board</param>
        /// <param name="latestMarkLocation">Latest mark location of a player</param>
        /// <param name="markSymbol">the mark symbol of the player</param>
        /// <returns>The location on the game board to win instantly or prevent the opponent to win instantly</returns>
        private Tuple<int, int> CheckInstantWin(
            Dictionary<int, int> gameBoard, 
            Tuple<int, int> latestMarkLocation, 
            int markSymbol)
        {
            // horizontal
            int offset = 0;
            while (latestMarkLocation.Item1 - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minX = latestMarkLocation.Item1 - offset + 1;
            offset = 0;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxX = latestMarkLocation.Item1 + offset - 1;
            var tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey(latestMarkLocation.Item2 * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[latestMarkLocation.Item2 * HyperParam.boardSide + minX + i] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindRowOf5InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, latestMarkLocation.Item2);
                    }
                    tempArr[i] = 0;
                }
            }

            // vertical
            offset = 0;
            while (latestMarkLocation.Item2 - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minY = latestMarkLocation.Item2 - offset + 1;
            offset = 0;
            while (latestMarkLocation.Item2 + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxY = latestMarkLocation.Item2 + offset - 1;
            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + latestMarkLocation.Item1))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[latestMarkLocation.Item1 + HyperParam.boardSide * (minY + i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindRowOf5InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(latestMarkLocation.Item1, minY + i);
                    }
                    tempArr[i] = 0;
                }
            }

            //diagonal line #1
            offset = 1;
            while (latestMarkLocation.Item1 - offset >= 0 &&
                latestMarkLocation.Item2 - offset >= 0 &&
                offset <= 6)
            {
                offset += 1;
            }
            minX = latestMarkLocation.Item1 - offset + 1;
            minY = latestMarkLocation.Item2 - offset + 1;
            offset = 1;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide &&
                latestMarkLocation.Item2 + offset <= HyperParam.boardSide &&
                offset <= 6)
            {
                offset += 1;
            }
            maxX = latestMarkLocation.Item1 + offset - 1;
            maxY = latestMarkLocation.Item2 + offset - 1;

            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (minY + i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindRowOf5InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, minY + i);
                    }
                    tempArr[i] = 0;
                }
            }

            //diagonal line #2
            offset = 1;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide &&
                latestMarkLocation.Item2 - offset >= 0 &&
                offset <= 6)
            {
                offset += 1;
            }
            maxX = latestMarkLocation.Item1 + offset - 1;
            minY = latestMarkLocation.Item2 - offset + 1;
            offset = 1;
            while (latestMarkLocation.Item1 - offset >= 0 &&
                latestMarkLocation.Item2 + offset <= HyperParam.boardSide &&
                offset <= 6)
            {
                offset += 1;
            }
            minX = latestMarkLocation.Item1 - offset + 1;
            maxY = latestMarkLocation.Item2 + offset - 1;

            tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((maxY - i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (maxY - i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindRowOf5InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, maxY - i);
                    }
                    tempArr[i] = 0;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a player can mark on a location that guarantee a win in the next his mark
        /// </summary>
        /// <param name="gameBoard">The dictionary represents the game board</param>
        /// <param name="latestMarkLocation">Latest mark location of a player</param>
        /// <param name="markSymbol">the mark symbol of the player</param>
        /// <returns>a location that sastify the desire decribed in "summary"</returns>
        private Tuple<int, int> CheckSemiInstantWin(
            Dictionary<int, int> gameBoard,
            Tuple<int, int> latestMarkLocation,
            int markSymbol)
        {
            // horizontal
            int offset = 0;
            while (latestMarkLocation.Item1 - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minX = latestMarkLocation.Item1 - offset + 1;
            offset = 0;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxX = latestMarkLocation.Item1 + offset - 1;
            var tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey(latestMarkLocation.Item2 * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[latestMarkLocation.Item2 * HyperParam.boardSide + minX + i] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindOpen2EndsRowOf4InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, latestMarkLocation.Item2);
                    }
                    tempArr[i] = 0;
                }
            }

            // vertical
            offset = 0;
            while (latestMarkLocation.Item2 - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minY = latestMarkLocation.Item2 - offset + 1;
            offset = 0;
            while (latestMarkLocation.Item2 + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxY = latestMarkLocation.Item2 + offset - 1;
            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + latestMarkLocation.Item1))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[latestMarkLocation.Item1 + HyperParam.boardSide * (minY + i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindOpen2EndsRowOf4InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(latestMarkLocation.Item1, minY + i);
                    }
                    tempArr[i] = 0;
                }
            }

            //diagonal line #1
            offset = 1;
            while (latestMarkLocation.Item1 - offset >= 0 &&
                latestMarkLocation.Item2 - offset >= 0 &&
                offset <= 6)
            {
                offset += 1;
            }
            minX = latestMarkLocation.Item1 - offset + 1;
            minY = latestMarkLocation.Item2 - offset + 1;
            offset = 1;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide &&
                latestMarkLocation.Item2 + offset <= HyperParam.boardSide &&
                offset <= 6)
            {
                offset += 1;
            }
            maxX = latestMarkLocation.Item1 + offset - 1;
            maxY = latestMarkLocation.Item2 + offset - 1;

            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (minY + i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindOpen2EndsRowOf4InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, minY + i);
                    }
                    tempArr[i] = 0;
                }
            }

            //diagonal line #2
            offset = 1;
            while (latestMarkLocation.Item1 + offset <= HyperParam.boardSide &&
                latestMarkLocation.Item2 - offset >= 0 &&
                offset <= 6)
            {
                offset += 1;
            }
            maxX = latestMarkLocation.Item1 + offset - 1;
            minY = latestMarkLocation.Item2 - offset + 1;
            offset = 1;
            while (latestMarkLocation.Item1 - offset >= 0 &&
                latestMarkLocation.Item2 + offset <= HyperParam.boardSide &&
                offset <= 6)
            {
                offset += 1;
            }
            minX = latestMarkLocation.Item1 - offset + 1;
            maxY = latestMarkLocation.Item2 + offset - 1;

            tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((maxY - i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (maxY - i)] == markSymbol)
                {
                    tempArr[i] = markSymbol;
                }
                else
                {
                    tempArr[i] = -1;
                }
            }
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (tempArr[i] == 0)
                {
                    tempArr[i] = markSymbol;
                    if (FindOpen2EndsRowOf4InOneDirection(tempArr))
                    {
                        return new Tuple<int, int>(minX + i, maxY - i);
                    }
                    tempArr[i] = 0;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a consecutive sequence of 4 identical marks in a line
        /// </summary>
        /// <param name="marksInALine">the array contains all marks of a line</param>
        /// <returns>true if found</returns>
        private bool FindOpen2EndsRowOf4InOneDirection(int[] marksInALine)
        {
            int maxLen = 0;
            int lowerBoundIndex = -1;
            int upperBoundIndex = -1;
            for(int i = 0; i < marksInALine.Length; ++i)
            {
                if (marksInALine[i] != 0 && marksInALine[i] != -1)
                {
                    maxLen += 1;
                    if (maxLen == 4)
                    {
                        upperBoundIndex = i + 1;
                        if(lowerBoundIndex >= 0 &&
                            upperBoundIndex < marksInALine.Length &&
                            marksInALine[upperBoundIndex] == 0)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    maxLen = 0;
                    if (marksInALine[i] == 0)
                        lowerBoundIndex = i;
                    else
                        lowerBoundIndex = -1;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds a consecutive sequence of 5 identical marks in a line
        /// </summary>
        /// <param name="marksInALine">the array contains all marks of a line</param>
        /// <returns>true if found</returns>
        private bool FindRowOf5InOneDirection(int[] marksInALine)
        {
            int maxLen = 0;
            for(int i = 0; i < marksInALine.Length; ++i)
            {
                if(marksInALine[i] != 0 && marksInALine[i] != -1)
                {
                    maxLen += 1;
                    if(maxLen == 5)
                    {
                        return true;
                    }
                }
                else
                {
                    maxLen = 0;
                }
            }
            return false;
        }

        /// <summary>
        /// The simple AI can win instantly and so does its opponent. Then it will try to attack
        /// </summary>
        /// <param name="gameBoard">the dictionary represents the gameboard</param>
        /// <param name="myMarkSymbol">the symbol of the current player</param>
        /// <param name="opponentMarkSymbol">the symbol of the current player's opponent</param>
        /// <param name="gameBoardMinX">min x of marks on game board</param>
        /// <param name="gameBoardMinY">min y of marks on game board</param>
        /// <param name="gameBoardMaxX">max x of marks on game board</param>
        /// <param name="gameBoardMaxY">max y of marks on game board</param>
        /// <returns>A "best" location to mark on with its score</returns>
        private Tuple<int, int, int> GreedySearch(
            Dictionary<int, int> gameBoard, 
            int myMarkSymbol,
            int opponentMarkSymbol,
            int gameBoardMinX,
            int gameBoardMinY,
            int gameBoardMaxX,
            int gameBoardMaxY)
        {
            var x1 = gameBoardMinX;
            while(x1 > 0 && x1 > gameBoardMinX - 2)
            {
                x1 -= 1;
            }
            var y1 = gameBoardMinY;
            while(y1 > 0 && y1 > gameBoardMinY - 2)
            {
                y1 -= 1;
            }
            var x2 = gameBoardMaxX;
            while (x2 < HyperParam.boardSide && x2 < gameBoardMaxX + 2)
            {
                x2 += 1;
            }
            var y2 = gameBoardMaxY;
            while (y2 < HyperParam.boardSide && y2 < gameBoardMaxY + 2)
            {
                y2 += 1;
            }

            int maxScore = 0;
            Tuple<int, int, int> nextMark = null;
            for(int i = y1; i <= y2; ++i)
            {
                for(int j = x1; j <= x2; ++j)
                {
                    if(!gameBoard.ContainsKey(Utilities.CoordinateToInteger(j, i)))
                    {
                        int currentScore = ComputeScoreForOneMark(gameBoard, j, i, myMarkSymbol, opponentMarkSymbol) ;
                        if (maxScore < currentScore)
                        {
                            maxScore = currentScore;
                            nextMark = new Tuple<int, int, int>(j, i, currentScore);
                        }
                    }
                }
            }

            return nextMark;
        }

        /// <summary>
        /// Computes total score for a potential mark on all 4 directions.
        /// </summary>
        /// <param name="gameBoard">the dictionary represents the gameboard</param>
        /// <param name="markLocationX">x of the potential mark location</param>
        /// <param name="markLocationY">y of the potential mark location</param>
        /// <param name="myMarkSymbol">the symbol of the current player</param>
        /// <param name="opponentMarkSymbol">the symbol of the current player's opponent</param>
        /// <returns>total score for the potential mark</returns>
        private int ComputeScoreForOneMark(
            Dictionary<int, int> gameBoard,
            int markLocationX, 
            int markLocationY, 
            int myMarkSymbol, 
            int opponentMarkSymbol)
        {
            int score = 0;

            // horizontal
            int offset = 0;
            while (markLocationX - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minX = markLocationX - offset + 1;
            offset = 0;
            while (markLocationX + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxX = markLocationX + offset - 1;
            var tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey(markLocationY * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[markLocationY * HyperParam.boardSide + minX + i] == myMarkSymbol)
                {
                    tempArr[i] = myMarkSymbol;
                }
                else
                {
                    tempArr[i] = opponentMarkSymbol;
                }
            }
            score += ComputeScoreForOneLineFromAMark(tempArr, markLocationX - minX,  myMarkSymbol, opponentMarkSymbol);

            // vertical
            offset = 0;
            while (markLocationY - offset >= 0 && offset <= 5)
            {
                offset += 1;
            }
            var minY = markLocationY - offset + 1;
            offset = 0;
            while (markLocationY + offset <= HyperParam.boardSide && offset <= 5)
            {
                offset += 1;
            }
            var maxY = markLocationY + offset - 1;
            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + markLocationX))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[markLocationX + HyperParam.boardSide * (minY + i)] == myMarkSymbol)
                {
                    tempArr[i] = myMarkSymbol;
                }
                else
                {
                    tempArr[i] = opponentMarkSymbol;
                }
            }
            score += ComputeScoreForOneLineFromAMark(tempArr, markLocationY - minY, myMarkSymbol, opponentMarkSymbol);

            //diagonal line #1
            offset = 0;
            while (markLocationX - offset >= 0 &&
                markLocationY - offset >= 0 &&
                offset <= 5)
            {
                offset += 1;
            }
            minX = markLocationX - offset + 1;
            minY = markLocationY - offset + 1;
            offset = 0;
            while (markLocationX + offset <= HyperParam.boardSide &&
                markLocationY + offset <= HyperParam.boardSide &&
                offset <= 5)
            {
                offset += 1;
            }
            maxX = markLocationX + offset - 1;
            maxY = markLocationY + offset - 1;

            tempArr = new int[maxY - minY + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((minY + i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (minY + i)] == myMarkSymbol)
                {
                    tempArr[i] = myMarkSymbol;
                }
                else
                {
                    tempArr[i] = opponentMarkSymbol;
                }
            }
            score += ComputeScoreForOneLineFromAMark(tempArr, markLocationX - minX, myMarkSymbol, opponentMarkSymbol);

            //diagonal line #2
            offset = 0;
            while (markLocationX + offset <= HyperParam.boardSide &&
                markLocationY - offset >= 0 &&
                offset <= 5)
            {
                offset += 1;
            }
            maxX = markLocationX + offset - 1;
            minY = markLocationY - offset + 1;
            offset = 0;
            while (markLocationX - offset >= 0 &&
                markLocationY + offset <= HyperParam.boardSide &&
                offset <= 5)
            {
                offset += 1;
            }
            minX = markLocationX - offset + 1;
            maxY = markLocationY + offset - 1;

            tempArr = new int[maxX - minX + 1];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                if (!gameBoard.ContainsKey((maxY - i) * HyperParam.boardSide + minX + i))
                {
                    tempArr[i] = 0;
                }
                else if (gameBoard[minX + i + HyperParam.boardSide * (maxY - i)] == myMarkSymbol)
                {
                    tempArr[i] = myMarkSymbol;
                }
                else
                {
                    tempArr[i] = opponentMarkSymbol;
                }
            }
            score += ComputeScoreForOneLineFromAMark(tempArr, markLocationX - minX, myMarkSymbol, opponentMarkSymbol);

            return score;
        }

        /// <summary>
        /// Computes score on one direction for a potential mark.
        /// This core is a part of total score.
        /// </summary>
        /// <param name="line">The array contains all marks and empty spots on a direction w.r.t the potential mark</param>
        /// <param name="markLocOnLine">The index of the potential mark on the array</param>
        /// <param name="myMarkSymbol">the current player's symbol</param>
        /// <param name="opponentMarkSymbol">The symbol of the current player's opponent</param>
        /// <returns>The partial core</returns>
        private int ComputeScoreForOneLineFromAMark(
            int[] line,
            int markLocOnLine,
            int myMarkSymbol,
            int opponentMarkSymbol)
        {
            int score = 0;
            int countMarks = 0;
            int countToSee5 = 0;
            line[markLocOnLine] = myMarkSymbol;
            for(int i = 0; i < line.Length; ++i)
            {
                if(line[i] == opponentMarkSymbol || i == line.Length - 1)
                {
                    if(countToSee5 >= 5)
                    {
                        score += countMarks * countMarks * countMarks * countMarks;
                        //score += countToSee5 - 5;
                    }
                    countMarks = 0;
                    countToSee5 = 0;
                }
                else
                {
                    countToSee5 += 1;
                    if(line[i] == myMarkSymbol)
                    {
                        countMarks += 1;
                    }
                }
            }

            countMarks = countToSee5 = 0;
            line[markLocOnLine] = opponentMarkSymbol;
            for (int i = 0; i < line.Length; ++i)
            {
                if (line[i] == myMarkSymbol || i == line.Length - 1)
                {
                    if (countToSee5 >= 5)
                    {
                        score += countMarks * countMarks * countMarks * countMarks;
                        score += countToSee5 - 5;
                    }
                    countMarks = 0;
                    countToSee5 = 0;
                }
                else
                {
                    countToSee5 += 1;
                    if (line[i] == opponentMarkSymbol)
                    {
                        countMarks += 1;
                    }
                }
            }

            return score;
        }

        #endregion

        #region Forward-Prunning Greedy Limited-Depth Search AI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameBoard"></param>
        /// <param name="opponentLatestMark"></param>
        /// <param name="myMarkSymbol"></param>
        /// <param name="opponentMarkSymbol"></param>
        /// <param name="gameBoardMinX"></param>
        /// <param name="gameBoardMinY"></param>
        /// <param name="gameBoardMaxX"></param>
        /// <param name="gameBoardMaxY"></param>
        /// <param name="currentTurn"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Tuple<int, int, int> ForwardPrunningAIReasoning(
            Dictionary<int, int> gameBoard,
            Tuple<int, int> opponentLatestMark,
            int myMarkSymbol,
            int opponentMarkSymbol,
            ref int gameBoardMinX,
            ref int gameBoardMinY,
            ref int gameBoardMaxX,
            ref int gameBoardMaxY,
            ref int currentTurn,
            int depth = HyperParam.reasoningDepth)
        {
            //Checks instance win for both players
            if (gameBoard.Count == 0)
            {
                return new Tuple<int, int, int>(HyperParam.boardSide / 2, HyperParam.boardSide / 2, int.MaxValue);
            }

            Tuple<int, int> nextMark = CheckInstantWin(gameBoard, LatestMarkLoc, myMarkSymbol);
            if (nextMark != null)
            {
                return new Tuple<int, int, int>(nextMark.Item1, nextMark.Item2, int.MaxValue);
            }

            nextMark = CheckInstantWin(gameBoard, opponentLatestMark, opponentMarkSymbol);
            if (nextMark != null)
            {
                return new Tuple<int, int, int>(nextMark.Item1, nextMark.Item2, 0);
            }

            nextMark = CheckSemiInstantWin(gameBoard, LatestMarkLoc, myMarkSymbol);
            if (nextMark != null)
            {
                return new Tuple<int, int, int>(nextMark.Item1, nextMark.Item2, int.MaxValue);
            }

            nextMark = CheckSemiInstantWin(gameBoard, opponentLatestMark, opponentMarkSymbol);
            if (nextMark != null)
            {
                return new Tuple<int, int, int>(nextMark.Item1, nextMark.Item2, 0);
            }

            //Finds the best mark for current player if there is no instance win
            var x1 = gameBoardMinX;
            while (x1 > 0 && x1 > gameBoardMinX - 2)
            {
                x1 -= 1;
            }
            var y1 = gameBoardMinY;
            while (y1 > 0 && y1 > gameBoardMinY - 2)
            {
                y1 -= 1;
            }
            var x2 = gameBoardMaxX;
            while (x2 < HyperParam.boardSide && x2 < gameBoardMaxX + 2)
            {
                x2 += 1;
            }
            var y2 = gameBoardMaxY;
            while (y2 < HyperParam.boardSide && y2 < gameBoardMaxY + 2)
            {
                y2 += 1;
            }

            //Forward prunning - Pruns branches with low scores
            List<Tuple<int, int, int>> nextMarks = new List<Tuple<int, int, int>>();
            for (int i = y1; i <= y2; ++i)
            {
                for (int j = x1; j <= x2; ++j)
                {
                    if (!gameBoard.ContainsKey(Utilities.CoordinateToInteger(j, i)))
                    {
                        int currentScore = ComputeScoreForOneMark(gameBoard, j, i, myMarkSymbol, opponentMarkSymbol);
                        if (currentScore >= HyperParam.searchThreshold)
                        {
                            nextMarks.Add(new Tuple<int, int, int>(j, i, currentScore));
                        }
                    }
                }
            }

            //If there is a leaf
            if(nextMarks.Capacity == 0)
            {
                return GreedySearch(
                    gameBoard,
                    myMarkSymbol,
                    opponentMarkSymbol,
                    gameBoardMinX,
                    gameBoardMinY,
                    gameBoardMaxX,
                    gameBoardMaxY);
            }
            //Or limited depth of search reached
            if(depth == 0)
            {
                return nextMarks.OrderByDescending(nMark => nMark.Item3).First();
            }

            //Else extending branches
            int currentGameBoardMinX = gameBoardMinX;
            int currentGameBoardMinY = gameBoardMinY;
            int currentGameBoardMaxX = gameBoardMaxX;
            int currentGameBoardMaxY = gameBoardMaxY;
            int currentPlayer = currentTurn;
            var newNextMarks = new List<Tuple<int, int, int>>();
            foreach (var nMark in nextMarks)
            {
                //Assumes that the current player mark on this location
                AIMarksOnGameBoard(
                    gameBoard,
                    nMark.Item1,
                    nMark.Item2,
                    myMarkSymbol,
                    ref gameBoardMinX,
                    ref gameBoardMinY,
                    ref gameBoardMaxX,
                    ref gameBoardMaxY);
                //Consider the opponent playing with greedy search strategy
                //to find the opponent's mark's location
                var opponentMark = GreedySearchAIReasoning(
                    gameBoard,
                    opponentLatestMark,
                    myMarkSymbol,
                    opponentMarkSymbol,
                    ref gameBoardMinX,
                    ref gameBoardMinY,
                    ref gameBoardMaxX,
                    ref gameBoardMaxY);
                //Assume the opponent would mark on this location
                AIMarksOnGameBoard(
                    gameBoard,
                    opponentMark.Item1,
                    opponentMark.Item2,
                    opponentMarkSymbol,
                    ref gameBoardMinX,
                    ref gameBoardMinY,
                    ref gameBoardMaxX,
                    ref gameBoardMaxY);
                //Go deeper to find a series of marks of current player and his opponents
                var tempMark = ForwardPrunningAIReasoning(
                    gameBoard,
                    opponentLatestMark,
                    myMarkSymbol,
                    opponentMarkSymbol,
                    ref gameBoardMinX,
                    ref gameBoardMinY,
                    ref gameBoardMaxX,
                    ref gameBoardMaxY,
                    ref currentTurn,
                    depth - 1);
                //return each mark's location of current player with the mark's score
                //add it to a list to find the best mark later
                newNextMarks.Add(new Tuple<int, int, int>(nMark.Item1, nMark.Item2, tempMark.Item3));
                //recover game's parameters
                //because all things happening here are just assumption
                gameBoardMinX = currentGameBoardMinX;
                gameBoardMinY = currentGameBoardMinY;
                gameBoardMaxX = currentGameBoardMaxX;
                gameBoardMaxY = currentGameBoardMaxY;
                gameBoard.Remove(opponentMark.Item2 * HyperParam.boardSide + opponentMark.Item1);
                gameBoard.Remove(nMark.Item2 * HyperParam.boardSide + nMark.Item1);
                currentTurn = currentPlayer;
            }
            //Returns best mark in the list
            return newNextMarks.OrderByDescending(nMark => nMark.Item3).First();
        }

        #endregion
    }
}
