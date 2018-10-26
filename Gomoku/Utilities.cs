using System;
using System.Windows;

namespace Gomoku
{
    /// <summary>
    /// utility static functions
    /// </summary>
    class Utilities
    {
        /// <summary>
        /// compute location of grid node which is nearest to mouse pointer
        /// when the mouse pointer is on the gameboard
        /// </summary>
        /// <param name="p">mouse pointer coordinates relative to gameboard canvas</param>
        /// <returns>coordinates of desired grid node relative to gameboard canvas</returns>
        public static Tuple<int, int> GetGameBoardLocationInPixelsFromMousePosition(Point p)
        {
            var x = (int)p.X;
            var y = (int)p.Y;
            var test = x % HyperParam.cellSide;
            x = test < HyperParam.cellSide / 2 ? (x / HyperParam.cellSide) * HyperParam.cellSide : (x / HyperParam.cellSide + 1) * HyperParam.cellSide;
            test = y % HyperParam.cellSide;
            y = test < HyperParam.cellSide / 2 ? (y / HyperParam.cellSide) * HyperParam.cellSide : (y / HyperParam.cellSide + 1) * HyperParam.cellSide;

            return new Tuple<int, int>(x, y);
        }

        public static void EndGameMessage(string message)
        {
            MessageBoxResult result = MessageBox.Show(message,
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
