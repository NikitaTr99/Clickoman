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
        private static readonly int col = 12;
        private static readonly int row = 18;

        private ApplicationContext context;

        private string current_player_name;
        private Player current_player;

        private RewardManager rewardManager;

        private Button[,] blocks;

        private List<Point> blocks_to_delete;

        private readonly SolidColorBrush[] colors =
        {
            Brushes.Green,
            Brushes.Red,
            Brushes.DodgerBlue,
            Brushes.Yellow
        };

        private Random random;

        private int scope;
        private int time;

        private DispatcherTimer timer;
        private DispatcherTimer check_timer;
        private DispatcherTimer stopwatch;
        
        int blocks_with_matches;

        public MainWindow()
        {
            InitializeComponent();
            current_player_name = "Unknown";
            init();
        }
        
        public MainWindow(string player_name)
        {
            InitializeComponent();
            current_player_name = player_name;
            init();
        }

        private void createField()
        {
            int x = 0, y = 0;

            scope = 0;
            time = 0;
            current_player.clear();

            GameField.Children.Clear();
            
            ScopeOut.Content = "Счёт: " + scope;

            StartButton.Content = "Заново";

            blocks = new Button[col, row];

            for (var i = 0; i < row; i++)
            {
                x = 0;

                for (var j = 0; j < col; j++)
                {
                    var block = new Button
                    {
                        Background = colors[random.Next(colors.Length)],
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
                    blocks[j, i] = block;

                    x += 25;
                }

                y += 25;
            }

            timer.Start();
            check_timer.Start();
            stopwatch.Start();
        }

        private void blockClick(object sender, EventArgs e)
        {
            var block = sender as Button;
            
            if (block.Background == null) return;

            blocks_to_delete.Clear();

            var coord_names = block.Name.Split('_');

            var x = int.Parse(coord_names[1]);
            var y = int.Parse(coord_names[2]);

            var point = new Point(x, y);

            var block_color = block.Background;

            findBlocksToDelete(point, block_color);

            if (blocks_to_delete.Count >= 2)
            {
                foreach (var p in blocks_to_delete)
                {
                    var b = blocks[Convert.ToInt32(p.X), Convert.ToInt32(p.Y)];
                    b.Background = null;
                    b.BorderBrush = null;
                }

                switch (blocks_to_delete.Count)
                {
                    case 5:
                        rewardManager.addReward("Уничтожить 5 блоков одновременно");
                        break;
                    case 7:
                        rewardManager.addReward("Уничтожить 7 блоков одновременно");
                        break;
                    case 10:
                        rewardManager.addReward("Уничтожить 10 блоков одновременно (редкий)");
                        break;
                }

                scope += blocks_to_delete.Count;
                ScopeOut.Content = "Счёт: " + scope;
            }
        }

        private List<Point> findBlocksAround(Point p)
        {
            var buffer = new List<Point>();

            if (p.X - 1 >= 0) buffer.Add(new Point(p.X - 1, p.Y));

            //Right
            if (p.X + 1 < col) buffer.Add(new Point(p.X + 1, p.Y));

            //Top
            if (p.Y - 1 >= 0) buffer.Add(new Point(p.X, p.Y - 1));

            //Bottom
            if (p.Y + 1 < row) buffer.Add(new Point(p.X, p.Y + 1));

            return buffer;
        }

        private void findBlocksToDelete(Point p, Brush blockColor)
        {
            if (!blocks_to_delete.Contains(p))
            {
                var block = blocks[Convert.ToInt32(p.X), Convert.ToInt32(p.Y)];
                if (block.Background == blockColor)
                {
                    blocks_to_delete.Add(p);

                    foreach (var point in findBlocksAround(p)) 
                        findBlocksToDelete(point, blockColor);
                }
            }
        }

        private void moveDown()
        {
            for (var i = 0; i < row; i++)
            for (var j = 0; j < col; j++)
                if (blocks[j, i].Background == null)
                    if (i - 1 >= 0)
                    {
                        blocks[j, i].Background = blocks[j, i - 1].Background;
                        blocks[j, i].BorderBrush = blocks[j, i - 1].BorderBrush;

                        blocks[j, i - 1].Background = null;
                        blocks[j, i - 1].BorderBrush = null;
                    }
        }
        
        private void isEndGame()
        {
            blocks_with_matches = 0;
            for (var i = 0; i < col; i++)
            for (var j = 0; j < row; j++)
                if (blocks[i, j].Background != null)
                {
                    var blocks_around = findBlocksAround(new Point(i, j));
                    if (blocks_around.Count > 0)
                    {
                        foreach (var b in blocks_around)
                        {
                            if (blocks[Convert.ToInt32(b.X), Convert.ToInt32(b.Y)].Background ==
                                blocks[i, j].Background)
                            {
                                blocks_with_matches++;
                            }
                        }
                    }
                }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            createField();
        }
        
        private void RewardButton_Click(object sender, RoutedEventArgs e)
        {
            RewardWindow rewardWindow = new RewardWindow(context, current_player.Id);
            rewardWindow.ShowDialog();
        }

        private void stopGame()
        {
            check_timer.Stop();
            stopwatch.Stop();

            current_player.Scope = scope;
            current_player.Time = time;

            if (time <= 20)
            {
                rewardManager.addReward("Закончить игру менее чем за 20 сек. (редкий)");
            }
             else if (time <= 25)
            {
                rewardManager.addReward("Закончить игру менее чем за 25 сек.");
            }
            else if (time <= 30)
            {
                rewardManager.addReward("Закончить игру менее чем за 30 сек.");
            }

            var player = context.Players.FirstOrDefault(p => p.Name == current_player.Name);
            
            if (player != null)
            {
                player.Scope = scope;
                player.Time = time;

                context.Entry(player).State = EntityState.Modified;
                
                rewardManager.pushRewards(player.Id);

            }
            else
            {
                throw new NullReferenceException("Player not found");
            }

            context.Players.Load();

            RatingTable.ItemsSource = context.Players.Local.ToBindingList();
            
            RatingTable.Items.Refresh();
            
            context.SaveChanges();
            
            MessageBox.Show("Конец игры. Счёт: " + scope);
        }

        private void updateTimerTick(object sender, EventArgs e)
        {
            moveDown();
        }
        
        private void checkTimerTick(object sender, EventArgs e)
        {
            isEndGame();
            if (blocks_with_matches <= 0)
            {
                stopGame();
            }
        }

        private void stopwatchTimerTick(object sender, EventArgs e)
        {
            time++;
        }

        private void init()
        {
            context = new ApplicationContext();
            rewardManager = new RewardManager(context);
            random = new Random();
            blocks_to_delete = new List<Point>();
            
            context.Players.Load();
            
            var player = context.Players.FirstOrDefault(p => p.Name == current_player_name);

            if (player != null)
            {
                current_player = player;
            }
            else
            {
                context.Players.Add(new Player(current_player_name, 0, 0));
                context.SaveChanges();
                
                player = context.Players.FirstOrDefault(p => p.Name == current_player_name);
                if (player != null)
                {
                    current_player = player;
                }
            }

            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };

            stopwatch = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 1, 0)
            };

            check_timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 300)
            };

            timer.Tick += updateTimerTick;
            check_timer.Tick += checkTimerTick;
            stopwatch.Tick += stopwatchTimerTick;

            RatingTable.ItemsSource = context.Players.Local.ToBindingList();

            PlayerNameOut.Content = current_player.Name;
        }
    }
}