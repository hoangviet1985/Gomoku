using System.Windows;
using System.Windows.Input;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            game = new Game(1, gameBoard);
        }

        private void GameBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (game.GameMode == 0)
            {
                var gameBoardPoint = Utilities.GetGameBoardLocationInPixelsFromMousePosition(Mouse.GetPosition(gameBoard));
                var playerActResult = game.HumanActing((gameBoardPoint.Item1 + 1) / HyperParam.cellSide, (gameBoardPoint.Item2 + 1) / HyperParam.cellSide);
                if (playerActResult)
                {
                    if (game.ActivePlayerID == (int)HyperParam.PlayerID.Player2)
                    {
                        Draw.DrawO(gameBoardPoint.Item1, gameBoardPoint.Item2, 2 * HyperParam.circleRadius, 2 * HyperParam.circleRadius, gameBoard);
                    }
                    else
                    {
                        Draw.DrawX(gameBoardPoint.Item1, gameBoardPoint.Item2, gameBoard);
                    }
                }
            }
            else if(game.GameMode == 1 && game.ActivePlayerID == (int)HyperParam.PlayerID.Player1)
            {
                var gameBoardPoint = Utilities.GetGameBoardLocationInPixelsFromMousePosition(Mouse.GetPosition(gameBoard));
                var playerActResult = game.HumanActing((gameBoardPoint.Item1 + 1) / HyperParam.cellSide, (gameBoardPoint.Item2 + 1) / HyperParam.cellSide);
                if (playerActResult)
                {
                    Draw.DrawO(gameBoardPoint.Item1, gameBoardPoint.Item2, 2 * HyperParam.circleRadius, 2 * HyperParam.circleRadius, gameBoard);
                }
            }
        }
    }
}
