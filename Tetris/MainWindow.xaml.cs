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
using System.Windows.Threading;

namespace Tetris
{
    

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        Board myBoard;
        public MainWindow()
        {
            InitializeComponent();
        }
        void MainWindow_Initialized(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 1);
            GameStart();
        }
        private void GameStart()
        {
            MainGrid.Children.Clear();
            myBoard = new Board(MainGrid);
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Score.Content = myBoard.getScore().ToString("00000000");
            Lines.Content = myBoard.getLines().ToString("00000000");
            myBoard.CurrentTetraminoMovDown();
        }

        private void GamePause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovRotate();
                    break;
                case Key.F2:
                    GameStart();
                    break;
                case Key.F3:
                    GamePause();
                    break;
                default:
                    break;
            }
        }
    }
}
