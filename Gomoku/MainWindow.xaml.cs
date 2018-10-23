using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void gameBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var gameBoardPoint = Utilities.getGameBoardXFromMousePosition(Mouse.GetPosition(gameBoard));
            if (HyperParam.turn)
            {
                draw.circle(gameBoardPoint.Item1, gameBoardPoint.Item2, 2 * HyperParam.circleRadius, 2 * HyperParam.circleRadius, gameBoard);
                HyperParam.turn = false;
            }
            else
            {
                draw.x(gameBoardPoint.Item1, gameBoardPoint.Item2, gameBoard);
                HyperParam.turn = true;
            }
        }
    }
}
