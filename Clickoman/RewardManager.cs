using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Clickoman
{
    public class RewardManager
    {
        private ApplicationContext context;

        private Stack<string> rewards;
        private Stack<Reward> rewards2;

        public RewardManager(ApplicationContext context)
        {
            this.context = context;
            rewards = new Stack<string>();
        }

        public void addReward(string name)
        {
            if (!rewards.Contains(name)) rewards.Push(name);
        }

        public void pushRewards(int player)
        {
            foreach (var s in rewards)
            {
                if (!context.Rewards.Where(rp => rp.Player == player).Select(r => r.Name == s).Contains(true))
                {
                    var reward = new Reward(s, player);
                    context.Rewards.AddOrUpdate(reward);
                }
            }
            context.SaveChanges();
            rewards.Clear();
        }
    }
}