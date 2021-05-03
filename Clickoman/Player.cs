using System.ComponentModel.DataAnnotations.Schema;

namespace Clickoman
{
    [Table("Player")]
    public class Player
    {

        public Player()
        {
        }

        public Player(string name, int scope, int time)
        {
            Name = name;
            Scope = scope;
            Time = time;
        }

        public Player(int id, string name, int scope, int time)
        {
            Id = id;
            Name = name;
            Scope = scope;
            Time = time;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int Scope { get; set; }

        public int Time { get; set; }

        public void clear()
        {
            Scope = 0;
            Time = 0;
        }
    }
}