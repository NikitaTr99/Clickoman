using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Clickoman
{
    [Table("Player")]
    public class Player
    {
        private int id;
        private string name;
        private int scope;
        private int time;

        public Player()
        {
        }

        public Player(string name, int scope, int time)
        {
            this.name = name;
            this.scope = scope;
            this.time = time;
        }

        public void clear()
        {
            this.scope = 0;
            this.time = 0;
        }

        public Player(int id, string name, int scope, int time)
        {
            this.id = id;
            this.name = name;
            this.scope = scope;
            this.time = time;
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

        public int Scope
        {
            get => scope;
            set => scope = value;
        }

        public int Time
        {
            get => time;
            set => time = value;
        }
    }
}