using System.Windows;
using System.Windows.Input;

namespace Gomoku
{
    /// <summary>
    /// Description: This class represent a GUI and interaction logic for MainWindow.xaml
    /// Author: Viet Dinh
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            MessageBoxManager.Cancel = "P vs. cAI";
            MessageBoxManager.Yes = "P vs. sAI";
            MessageBoxManager.No = "sAI vs. cAI";
            MessageBoxManager.Register();
            var result = MessageBox.Show("Please choose game mode!", "Gomoku", MessageBoxButton.YesNoCancel);
            int chosenGameMode = 0;
            if (result == MessageBoxResult.Yes)
            {
                chosenGameMode = 1;
                gameModeLb.Content = "Person vs. simple AI";
            }
            else if (result == MessageBoxResult.No)
            {
                chosenGameMode = 2;
                gameModeLb.Content = "simple AI vs. complex AI";
            }
            else if (result == MessageBoxResult.Cancel)
            {
                chosenGameMode = 3;
                gameModeLb.Content = "Person vs. complex AI";
            }
            game = new Game(chosenGameMode, gameBoard, currentPlayerCv);

            Draw.DrawO((int)currentPlayerCv.Width / 2,
                        (int)currentPlayerCv.Height / 2,
                        2 * HyperParam.circleRadius,
                        2 * HyperParam.circleRadius,
                        currentPlayerCv);
        }

        private void GameBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (game.GameMode == 0)
            {
                var gameBoardPoint = Utilities.GetGameBoardLocationInPixelsFromMousePosition(Mouse.GetPosition(gameBoard));
                var playerActResult = game.HumanActing((gameBoardPoint.Item1 + 1) / HyperParam.cellSide, (gameBoardPoint.Item2 + 1) / HyperParam.cellSide);
                if (playerActResult)
                {
                    if (game.activePlayerID == (int)HyperParam.PlayerID.Player2)
                    {
                        Draw.DrawO(gameBoardPoint.Item1, gameBoardPoint.Item2, 2 * HyperParam.circleRadius, 2 * HyperParam.circleRadius, gameBoard);
                        currentPlayerCv.Children.Clear();
                        Draw.DrawX((int)currentPlayerCv.Width / 2,
                        (int)currentPlayerCv.Height / 2,
                        currentPlayerCv);
                    }
                    else
                    {
                        Draw.DrawX(gameBoardPoint.Item1, gameBoardPoint.Item2, gameBoard);
                        currentPlayerCv.Children.Clear();
                        Draw.DrawO((int)currentPlayerCv.Width / 2,
                        (int)currentPlayerCv.Height / 2,
                        2 * HyperParam.circleRadius,
                        2 * HyperParam.circleRadius,
                        currentPlayerCv);
                    }
                }
            }
            else if((game.GameMode == 1 || game.GameMode == 3) && 
                game.activePlayerID == (int)HyperParam.PlayerID.Player1)
            {
                var gameBoardPoint = Utilities.GetGameBoardLocationInPixelsFromMousePosition(Mouse.GetPosition(gameBoard));
                var playerActResult = game.HumanActing((gameBoardPoint.Item1 + 1) / HyperParam.cellSide, (gameBoardPoint.Item2 + 1) / HyperParam.cellSide);
                if (playerActResult)
                {
                    Draw.DrawO(gameBoardPoint.Item1, gameBoardPoint.Item2, 2 * HyperParam.circleRadius, 2 * HyperParam.circleRadius, gameBoard);
                    currentPlayerCv.Children.Clear();
                    Draw.DrawX((int)currentPlayerCv.Width / 2,
                    (int)currentPlayerCv.Height / 2,
                    currentPlayerCv);
                }
            }
        }
    }
}
