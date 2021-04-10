using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Domain
{
    public class Samurai
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //One to many :- Samurai : Quotes
        public List<Quote> Quotes { get; set; } = new List<Quote>();

        //Many to many :- Samurais : Battles
        public List<Battle> Battles { get; set; } = new List<Battle>();

        //One to one :- Samurai : Horse
        public Horse Horse { get; set; }
    }
}
