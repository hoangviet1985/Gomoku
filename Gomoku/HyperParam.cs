namespace Gomoku
{
    /// <summary>
    /// common parameters
    /// </summary>
    class HyperParam
    {
        /// <summary>
        /// radius of symbol "o" in pixels
        /// </summary>
        public static int circleRadius = 10;
        /// <summary>
        /// a cell is a small square on gameboard
        /// cell side is measured in number of pixels
        /// </summary>
        public static int cellSide = 30; 
        /// <summary>
        /// gameboard side is measured in number of cells
        /// </summary>
        public static int boardSide = 20;
        /// <summary>
        /// players' IDs
        /// </summary>
        public enum PlayerID { Player1 = 1, Player2 };
        /// <summary>
        /// used to save to the dictionary representing inner state of gameboard
        /// </summary>
        public enum MarkSymbol { Player1 = 1, Player2};
    }
}
