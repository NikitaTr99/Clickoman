using System.ComponentModel.DataAnnotations.Schema;

namespace Clickoman
{
    [Table("Reward")]
    public class Reward
    {

        public Reward()
        {
        }

        public Reward(string name)
        {
            Name = name;
        }

        public Reward(string name, int player)
        {
            Name = name;
            Player = player;
        }

        public Reward(int id, string name, int player)
        {
            Id = id;
            Name = name;
            Player = player;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Player { get; set; }
    }
}