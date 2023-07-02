using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        int columnCount = 16;
        int rowCount = 16;
        ControlTemplate roundLabel;

        Game game;

        public MainWindow()
        {
            InitializeComponent();

            FillGrid();
            roundLabel = (ControlTemplate)(this.Resources["roundLabel"]);
            game = new Game(grid, points, start, roundLabel);

            pause.Checked += (s,e) => game.Pause();
            pause.Unchecked += (s, e) => game.Continue();
            start.Click += Start_Click;
        }

        void FillGrid()
        {
            if(grid is null)
            {
                grid = new Grid();
            }
            else
            {
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
                grid.Children.Clear();
            }

            for(int i = 0; i < columnCount; i++)
            {
                grid.ColumnDefinitions.Add(new()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            for(int i = 0; i < rowCount; i++)
            {
                grid.RowDefinitions.Add(new()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            game.MoveDirection = e.Key switch
            {
                Key.Left => Direction.Left,
                Key.Up => Direction.Up,
                Key.Right => Direction.Right,
                Key.Down => Direction.Down
            };
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            points.Content = "0";
            start.Visibility=Visibility.Hidden;
            game = new Game(grid, points, start, roundLabel);
        }
    }
}
