using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication7.Code
{
    public class Player
    {
        public string Name = "";

        public List<string> Throws = new List<string>();
        public Player(string name)
        {
            Name = name;
        }
    }
}
