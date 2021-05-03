using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Clickoman
{
    public class RewardManager
    {
        private readonly ApplicationContext _context;

        private readonly Stack<string> _rewards;

        public RewardManager(ApplicationContext context)
        {
            _context = context;
            _rewards = new Stack<string>();
        }

        public void addReward(string name)
        {
            if (!_rewards.Contains(name)) _rewards.Push(name);
        }

        public void pushRewards(int player)
        {
            foreach (var s in _rewards)
                if (!_context.Rewards.Where(rp => rp.Player == player).Select(r => r.Name == s).Contains(true))
                {
                    var reward = new Reward(s, player);
                    _context.Rewards.AddOrUpdate(reward);
                }
            _context.SaveChanges();
            _rewards.Clear();
        }
    }
}