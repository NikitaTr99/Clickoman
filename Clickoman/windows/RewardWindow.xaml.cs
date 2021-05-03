using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Clickoman.windows
{
    public partial class RewardWindow : Window
    {

        private readonly ApplicationContext _context;

        public RewardWindow(ApplicationContext ac, int player)
        {
            InitializeComponent();
            _context = ac;

            _context.Rewards.Load();

            var rewards = _context.Rewards.Where(r => r.Player == player).ToList();

            RewardTable.ItemsSource = rewards;
        }
    }
}