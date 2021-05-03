using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Clickoman.windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int Col = 12;
        private static readonly int Row = 18;

        private readonly SolidColorBrush[] _colors =
        {
            Brushes.Green,
            Brushes.Red,
            Brushes.DodgerBlue,
            Brushes.Yellow
        };

        private Button[,] _blocks;

        private List<Point> _blocksToDelete;

        private int _blocksWithMatches;
        private DispatcherTimer _checkTimer;

        private ApplicationContext _context;
        private Player _currentPlayer;

        private readonly string _currentPlayerName;

        private Random _random;

        private RewardManager _rewardManager;

        private int _scope;
        private DispatcherTimer _stopwatch;
        private int _time;

        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _currentPlayerName = "Unknown";
            init();
        }

        public MainWindow(string playerName)
        {
            InitializeComponent();
            _currentPlayerName = playerName;
            init();
        }

        private void createField()
        {
            int x = 0, y = 0;

            _scope = 0;
            _time = 0;
            _currentPlayer.clear();

            GameField.Children.Clear();

            ScopeOut.Content = "Счёт: " + _scope;

            StartButton.Content = "Заново";

            _blocks = new Button[Col, Row];

            for (var i = 0; i < Row; i++)
            {
                x = 0;

                for (var j = 0; j < Col; j++)
                {
                    var block = new Button
                    {
                        Background = _colors[_random.Next(_colors.Length)],
                        BorderBrush = Brushes.Black,
                        Name = "block_" + j + "_" + i,
                        Height = 25,
                        Width = 25
                    };

                    block.Style = FindResource("BlockStyle") as Style;

                    block.Click += blockClick;

                    Canvas.SetLeft(block, x);
                    Canvas.SetTop(block, y);

                    GameField.Children.Add(block);
                    _blocks[j, i] = block;

                    x += 25;
                }

                y += 25;
            }

            _timer.Start();
            _checkTimer.Start();
            _stopwatch.Start();
        }

        private void blockClick(object sender, EventArgs e)
        {
            var block = sender as Button;

            if (block.Background == null) return;

            _blocksToDelete.Clear();

            var coordNames = block.Name.Split('_');

            var x = int.Parse(coordNames[1]);
            var y = int.Parse(coordNames[2]);

            var point = new Point(x, y);

            var blockColor = block.Background;

            findBlocksToDelete(point, blockColor);

            if (_blocksToDelete.Count >= 2)
            {
                foreach (var p in _blocksToDelete)
                {
                    var b = _blocks[Convert.ToInt32(p.X), Convert.ToInt32(p.Y)];
                    b.Background = null;
                    b.BorderBrush = null;
                }

                switch (_blocksToDelete.Count)
                {
                    case 5:
                        _rewardManager.addReward("Уничтожить 5 блоков одновременно");
                        break;
                    case 7:
                        _rewardManager.addReward("Уничтожить 7 блоков одновременно");
                        break;
                    case 10:
                        _rewardManager.addReward("Уничтожить 10 блоков одновременно (редкий)");
                        break;
                }

                _scope += _blocksToDelete.Count;
                ScopeOut.Content = "Счёт: " + _scope;
            }
        }

        private List<Point> findBlocksAround(Point p)
        {
            var buffer = new List<Point>();

            if (p.X - 1 >= 0) buffer.Add(new Point(p.X - 1, p.Y));

            //Right
            if (p.X + 1 < Col) buffer.Add(new Point(p.X + 1, p.Y));

            //Top
            if (p.Y - 1 >= 0) buffer.Add(new Point(p.X, p.Y - 1));

            //Bottom
            if (p.Y + 1 < Row) buffer.Add(new Point(p.X, p.Y + 1));

            return buffer;
        }

        private void findBlocksToDelete(Point p, Brush blockColor)
        {
            if (!_blocksToDelete.Contains(p))
            {
                var block = _blocks[Convert.ToInt32(p.X), Convert.ToInt32(p.Y)];
                if (block.Background == blockColor)
                {
                    _blocksToDelete.Add(p);

                    foreach (var point in findBlocksAround(p))
                        findBlocksToDelete(point, blockColor);
                }
            }
        }

        private void moveDown()
        {
            for (var i = 0; i < Row; i++)
            for (var j = 0; j < Col; j++)
                if (_blocks[j, i].Background == null)
                    if (i - 1 >= 0)
                    {
                        _blocks[j, i].Background = _blocks[j, i - 1].Background;
                        _blocks[j, i].BorderBrush = _blocks[j, i - 1].BorderBrush;

                        _blocks[j, i - 1].Background = null;
                        _blocks[j, i - 1].BorderBrush = null;
                    }
        }

        private void isEndGame()
        {
            _blocksWithMatches = 0;
            for (var i = 0; i < Col; i++)
            for (var j = 0; j < Row; j++)
                if (_blocks[i, j].Background != null)
                {
                    var blocksAround = findBlocksAround(new Point(i, j));
                    if (blocksAround.Count > 0)
                        foreach (var b in blocksAround)
                            if (_blocks[Convert.ToInt32(b.X), Convert.ToInt32(b.Y)].Background ==
                                _blocks[i, j].Background)
                                _blocksWithMatches++;
                }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            createField();
        }

        private void RewardButton_Click(object sender, RoutedEventArgs e)
        {
            var rewardWindow = new RewardWindow(_context, _currentPlayer.Id);
            rewardWindow.ShowDialog();
        }

        private void stopGame()
        {
            _checkTimer.Stop();
            _stopwatch.Stop();

            _currentPlayer.Scope = _scope;
            _currentPlayer.Time = _time;

            if (_time <= 20) _rewardManager.addReward("Закончить игру менее чем за 20 сек. (редкий)");
            else if (_time <= 25) _rewardManager.addReward("Закончить игру менее чем за 25 сек.");
            else if (_time <= 30) _rewardManager.addReward("Закончить игру менее чем за 30 сек.");

            var player = _context.Players.FirstOrDefault(p => p.Name == _currentPlayer.Name);

            if (player != null)
            {
                player.Scope = _scope;
                player.Time = _time;

                _context.Entry(player).State = EntityState.Modified;

                _rewardManager.pushRewards(player.Id);

            }
            else
            {
                throw new NullReferenceException("Player not found");
            }

            _context.Players.Load();

            RatingTable.ItemsSource = _context.Players.Local.ToBindingList();

            RatingTable.Items.Refresh();

            _context.SaveChanges();

            MessageBox.Show("Конец игры. Счёт: " + _scope);
        }

        private void updateTimerTick(object sender, EventArgs e)
        {
            moveDown();
        }

        private void checkTimerTick(object sender, EventArgs e)
        {
            isEndGame();
            if (_blocksWithMatches <= 0) stopGame();
        }

        private void stopwatchTimerTick(object sender, EventArgs e)
        {
            _time++;
        }

        private void init()
        {
            _context = new ApplicationContext();
            _random = new Random();
            _rewardManager = new RewardManager(_context);
            _blocksToDelete = new List<Point>();

            _context.Players.Load();

            var player = _context.Players.FirstOrDefault(p => p.Name == _currentPlayerName);

            if (player != null)
            {
                _currentPlayer = player;
            }
            else
            {
                _context.Players.Add(new Player(_currentPlayerName, 0, 0));
                _context.SaveChanges();

                player = _context.Players.FirstOrDefault(p => p.Name == _currentPlayerName);
                if (player != null) _currentPlayer = player;
            }

            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };

            _stopwatch = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 1, 0)
            };

            _checkTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 300)
            };

            _timer.Tick += updateTimerTick;
            _checkTimer.Tick += checkTimerTick;
            _stopwatch.Tick += stopwatchTimerTick;

            RatingTable.ItemsSource = _context.Players.Local.ToBindingList();

            PlayerNameOut.Content = _currentPlayer.Name;
        }
    }
}