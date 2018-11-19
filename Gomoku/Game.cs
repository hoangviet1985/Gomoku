using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gomoku
{
    /// <summary>
    /// a game
    /// </summary>
    class Game
    {
        /// <summary>
        /// represents inner states of the gameboard.
        /// when a player marks on the gameboard, his mark symbol (1 or 2)
        /// will be saved to the dictionary if it is a legal mark
        /// the key of the dictionary represent a grid node's location on the game board
        /// e.g grid node [1, 5] will be represented by the key of (1 x gameboard side + 5) = 25
        /// with the gameboard side set to 20.
        /// </summary>
        private Dictionary<int, int> gameBoard;

        /// <summary>
        /// contains 2 players
        /// </summary>
        private Player[] players;

        /// <summary>
        /// most top-left and bottom-right marks on the game board
        /// </summary>
        public int minX, minY, maxX, maxY;

        /// <summary>
        /// 3 game modes: 0 - person vs person, 1 - person vs machine, 2 - machine vs machine
        /// </summary>
        public int GameMode { get; set; }

        /// <summary>
        /// ID of the active player (1 or 2) who is taking turn
        /// </summary>
        public int ActivePlayerID { get; set; }

        /// <summary>
        /// the UI canvas for the AI to mark on
        /// </summary>
        private Canvas graphicalBoard;

        public Game(int gameMode, Canvas graphicalBoard)
        {
            gameBoard = new Dictionary<int, int>();
            GameMode = gameMode;
            maxX = maxY = -1;
            minX = minY = int.MaxValue;
            players = new Player[2];
            players[0] = new Player();
            players[1] = new Player();
            ActivePlayerID = (int)HyperParam.PlayerID.Player1;
            this.graphicalBoard = graphicalBoard;
            if(GameMode == 1 || GameMode == 2)
            {
                RunGameMode12();
            }
        }

        /// <summary>
        /// Threading function for AI
        /// </summary>
        private void RunGameMode12()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    AIActing();
                    await Task.Delay(1000);
                };
            });
        }
        
        /// <summary>
        /// Active player take action to mark on the gameboard
        /// </summary>
        /// <param name="x">x of grid node that the active player decides to mark on</param>
        /// <param name="y">y of grid node that the active player decides to mark on</param>
        /// <returns>result of active player's action</returns>
        public bool HumanActing(int x, int y)
        {
            if(ActivePlayerID == (int)HyperParam.PlayerID.Player1)
            {
                if(players[0].HumanMarksOnGameBoard(
                    gameBoard, 
                    x,
                    y,
                    (int)HyperParam.MarkSymbol.Player1,
                    ref minX,
                    ref minY,
                    ref maxX,
                    ref maxY))
                {
                    CheckGameEnd();
                    ActivePlayerID = (int)HyperParam.PlayerID.Player2;
                    return true;
                }
            }
            else if(ActivePlayerID == (int)HyperParam.PlayerID.Player2)
            {
                if (players[1].HumanMarksOnGameBoard(
                    gameBoard, 
                    x,
                    y,
                    (int)HyperParam.MarkSymbol.Player2,
                    ref minX,
                    ref minY,
                    ref maxX,
                    ref maxY))
                {
                    CheckGameEnd();
                    ActivePlayerID = (int)HyperParam.PlayerID.Player1;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// AI reasons and marks on game board
        /// </summary>
        public void AIActing()
        {
            if (ActivePlayerID == (int)HyperParam.PlayerID.Player1 &&
                GameMode == 2)
            {
                var nextMark = players[0].SimpleAIReasoning(
                    gameBoard, 
                    players[1].LatestMarkLoc, 
                    (int)HyperParam.MarkSymbol.Player1, 
                    (int)HyperParam.MarkSymbol.Player2,
                    minX,
                    minY,
                    maxX,
                    maxY);
                players[0].AIMarksOnGameBoard(
                    gameBoard, 
                    nextMark.Item1,
                    nextMark.Item2, 
                    (int)HyperParam.MarkSymbol.Player1,
                    ref minX,
                    ref minY,
                    ref maxX,
                    ref maxY);
                graphicalBoard.Dispatcher.Invoke(() =>
                {
                    Draw.DrawO(nextMark.Item1 * HyperParam.cellSide,
                    nextMark.Item2 * HyperParam.cellSide,
                    2 * HyperParam.circleRadius,
                    2 * HyperParam.circleRadius,
                    graphicalBoard);
                });
                CheckGameEnd();
                ActivePlayerID = (int)HyperParam.PlayerID.Player2;
            }
            else if (ActivePlayerID == (int)HyperParam.PlayerID.Player2 &&
                (GameMode == 1 || GameMode == 2))
            {
                var nextMark = players[1].SimpleAIReasoning(
                    gameBoard,
                    players[0].LatestMarkLoc,
                    (int)HyperParam.MarkSymbol.Player2,
                    (int)HyperParam.MarkSymbol.Player1,
                    minX,
                    minY,
                    maxX,
                    maxY);
                players[1].AIMarksOnGameBoard(
                    gameBoard, 
                    nextMark.Item1, 
                    nextMark.Item2, 
                    (int)HyperParam.MarkSymbol.Player2,
                    ref minX,
                    ref minY,
                    ref maxX,
                    ref maxY);
                graphicalBoard.Dispatcher.Invoke(() =>
                {
                    Draw.DrawX(nextMark.Item1 * HyperParam.cellSide,
                    nextMark.Item2 * HyperParam.cellSide,
                    graphicalBoard);
                });
                CheckGameEnd();
                ActivePlayerID = (int)HyperParam.PlayerID.Player1;
            }
        }

        /// <summary>
        /// check if a player wins or the gameboar is full
        /// </summary>
        /// <returns>game end value</returns>
        private bool CheckGameEnd()
        {
            // check on horizental direction with respect to newest mark of the active player
            var maxSymbolLength = 1;
            for(int x = 1; x <= 4; ++x)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 - x;
                if (nextX < 0)
                {
                    break;
                }
                if(!gameBoard.ContainsKey(players[ActivePlayerID - 1].LatestMarkLoc.Item2 * HyperParam.boardSide + nextX) ||
                    gameBoard[players[ActivePlayerID - 1].LatestMarkLoc.Item2 * HyperParam.boardSide + nextX] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
            }
            if (maxSymbolLength == 5)
            {
                Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                return true;
            }
            for (int x = 1; x <= 4; ++x)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 + x;
                if (nextX > HyperParam.boardSide)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(players[ActivePlayerID - 1].LatestMarkLoc.Item2 * HyperParam.boardSide + nextX) ||
                    gameBoard[players[ActivePlayerID - 1].LatestMarkLoc.Item2 * HyperParam.boardSide + nextX] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
                if (maxSymbolLength == 5)
                {
                    Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                    return true;
                }
            }

            // check on vertical direction with respect to newest mark of the active player
            maxSymbolLength = 1;
            for (int y = 1; y <= 4; ++y)
            {
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 - y;
                if (nextY > HyperParam.boardSide)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(players[ActivePlayerID - 1].LatestMarkLoc.Item1 + HyperParam.boardSide * nextY) ||
                    gameBoard[players[ActivePlayerID - 1].LatestMarkLoc.Item1 + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
            }
            if (maxSymbolLength == 5)
            {
                Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                return true;
            }
            for (int y = 1; y <= 4; ++y)
            {
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 + y;
                if (nextY > HyperParam.boardSide)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(players[ActivePlayerID - 1].LatestMarkLoc.Item1 + HyperParam.boardSide * nextY) ||
                    gameBoard[players[ActivePlayerID - 1].LatestMarkLoc.Item1 + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
                if (maxSymbolLength == 5)
                {
                    Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                    return true;
                }
            }

            // check on first diagonal direction with respect to newest mark of the active player
            maxSymbolLength = 1;
            for (int xy = 1; xy <= 4; ++xy)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 - xy;
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 - xy;
                if (nextX < 0 || nextY < 0)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(nextX + HyperParam.boardSide * nextY) ||
                    gameBoard[nextX + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
            }
            if (maxSymbolLength == 5)
            {
                Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                return true;
            }
            for (int xy = 1; xy <= 4; ++xy)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 + xy;
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 + xy;
                if (nextX > HyperParam.boardSide || nextY > HyperParam.boardSide)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(nextX + HyperParam.boardSide * nextY) ||
                    gameBoard[nextX + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
                if (maxSymbolLength == 5)
                {
                    Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                    return true;
                }
            }

            // check on second diagonal direction with respect to newest mark of the active player
            maxSymbolLength = 1;
            for (int xy = 1; xy <= 4; ++xy)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 - xy;
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 + xy;
                if (nextX < 0 || nextY > HyperParam.boardSide)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(nextX + HyperParam.boardSide * nextY) ||
                    gameBoard[nextX + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
            }
            if (maxSymbolLength == 5)
            {
                Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                return true;
            }
            for (int xy = 1; xy <= 4; ++xy)
            {
                var nextX = players[ActivePlayerID - 1].LatestMarkLoc.Item1 + xy;
                var nextY = players[ActivePlayerID - 1].LatestMarkLoc.Item2 - xy;
                if (nextX > HyperParam.boardSide || nextY < 0)
                {
                    break;
                }
                if (!gameBoard.ContainsKey(nextX + HyperParam.boardSide * nextY) ||
                    gameBoard[nextX + HyperParam.boardSide * nextY] != ActivePlayerID)
                {
                    break;
                }
                maxSymbolLength += 1;
                if (maxSymbolLength == 5)
                {
                    Utilities.EndGameMessage("Player " + ActivePlayerID + " won!");
                    return true;
                }
            }

            // when the gameboard is full and no players wins
            if (gameBoard.Count == HyperParam.boardSide * HyperParam.boardSide)
            {
                Utilities.EndGameMessage("Game draw!");
                return true;
            }

            // when the game does not end
            return false;
        }
    }
}
