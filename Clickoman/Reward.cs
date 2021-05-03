using System.ComponentModel.DataAnnotations.Schema;

namespace Clickoman
{
    [Table("Reward")]
    public class Reward
    {
        private int id;
        private string name;
        private int player;

        public Reward()
        {
        }

        public Reward(string name)
        {
            this.name = name;
        }

        public Reward(string name, int player)
        {
            this.name = name;
            this.player = player;
        }

        public Reward(int id, string name, int player)
        {
            this.id = id;
            this.name = name;
            this.player = player;
        }

        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Player
        {
            get => player;
            set => player = value;
        }
    }
}