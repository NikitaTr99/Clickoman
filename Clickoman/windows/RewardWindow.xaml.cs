using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Clickoman.windows
{
    public partial class RewardWindow : Window
    {

        private ApplicationContext context;
        
        public RewardWindow(ApplicationContext ac, int player)
        {
            InitializeComponent();
            context = ac;
            
            context.Rewards.Load();
            
            var rewards = context.Rewards.Where(r => r.Player == player).ToList();

            RewardTable.ItemsSource = rewards;
        }
    }
}