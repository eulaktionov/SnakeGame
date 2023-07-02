using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SnakeGame
{
    enum Direction { None, Left, Up, Right, Down };

    internal class Game
    {
        Random random = new Random();
        DispatcherTimer timer;
        int interval = 500;

        Grid grid;
        int columnCount;
        int rowCount;
        Label pointLabel;
        Control start;

        Brush snakeBrush = Brushes.Blue;
        Brush foodBrush = Brushes.Green;

        Snake snake;
        Cell food;

        public Direction MoveDirection = Direction.Right;
        public void Continue() => timer.Start();
        public void Pause() => timer.Stop();
        int points;

        ControlTemplate roundLabel;

        Place RandomPlace => new()
        {
            Col = random.Next(rowCount),
            Row = random.Next(rowCount)
        }; 


        public Game(Grid grid, Label pointLabel, Control start,
            ControlTemplate roundLabel) 
        { 
            this.grid = grid;
            grid.Children.Clear();
            columnCount = grid.ColumnDefinitions.Count;
            rowCount = grid.RowDefinitions.Count;

            this.pointLabel = pointLabel;
            points = 1;
            this.start = start;
            this.roundLabel = roundLabel;

            snake = CreateSnake();
            AddToGrid(snake);

            food = CreateFood();
            AddToGrid(food);

            timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(interval),
                IsEnabled = true
            };
            timer.Tick += (s, e) => MoveSnake();
        }

        Snake CreateSnake() => new Snake()
        {
            Place = RandomPlace,
            Brush = snakeBrush
        };

        Cell CreateFood()
        {
            Cell cell;
            do
            {
                cell = new Cell
                {
                    Place = RandomPlace,
                    Background = foodBrush,
                    Template = roundLabel
                };
            }
            while(snake.Includes(cell.Place));

            return cell;
        }

        void AddToGrid(Cell cell)
        {
            Grid.SetColumn(cell, cell.Place.Col);
            Grid.SetRow(cell, cell.Place.Row);
            grid.Children.Add(cell);
        }

        void AddToGrid(Snake snake)
        {
            foreach(var cell in snake.Body)
            {
                AddToGrid(cell);    
            }
        }

        void RemoveFromGrid(Snake snake)
        {
            foreach(var cell in snake.Body)
            {
                grid.Children.Remove(cell);
            }
        }

        public void MoveSnake()
        {
            var snakePlace = snake.Place;
            switch(MoveDirection)
            {
                case Direction.Left: snakePlace.Col--; break;
                case Direction.Up: snakePlace.Row--; break;
                case Direction.Right: snakePlace.Col++; break;
                case Direction.Down: snakePlace.Row++; break;
            }

            if(OutOfGrid(snakePlace)
            || snake.Includes(snakePlace))
            {
                EndGame();
                return;
            }

            RemoveFromGrid(snake);

            if(snakePlace == food.Place)
            {
                grid.Children.Remove(food);
                food = CreateFood();
                AddToGrid(food);

                Cell cell = new Cell()
                {
                    Background = snakeBrush
                };
                snake.Body.Add(cell);
                pointLabel.Content = (++points).ToString();
            }

            for(int i = snake.Body.Count - 1; i > 0; i--)
            {
                snake.Body[i].Place = snake.Body[i - 1].Place;
            }
            snake.Place = snakePlace;

            AddToGrid(snake);
        }

        bool OutOfGrid(Place place) =>
            OutOfRange(0, columnCount - 1, place.Col) ||
            OutOfRange(0, rowCount - 1, place.Row);

        static bool OutOfRange(int start, int end, int value) =>
            value < start || value > end;

        void EndGame()
        {
            MessageBox.Show("You lost!");
            timer.Stop();
            start.Visibility = Visibility.Visible;
        }
    }

    class Snake
    {
        public List<Cell> Body;
        Cell header;

        public Brush Brush
        {
            get => header.Background;
            set => header.Background = value;
        }
        public Place Place
        {
            get => header.Place;
            set => header.Place = value;
        }

        public Snake()
        {
            header = new();
            Body = new List<Cell>();
            Body.Add(header);
        }

        public bool Includes(Place place) =>
            Body.Exists(cell => cell.Place == place);
    }

    internal class Cell : Label
    {
        public Place Place { get; set; }
    }

    public struct Place
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public static bool operator ==(Place place1, Place place2)
            => place1.Col == place2.Col && place1.Row == place2.Row;
        public static bool operator !=(Place place1, Place place2)
            => !(place1 == place2);
    } 
}
