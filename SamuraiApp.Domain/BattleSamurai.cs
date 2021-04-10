using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Domain
{
    //EF core has already generated class BattleSamurai on it's own for many to many relationship.
    //But, now, we have to save some extra info (payload), to this table, apart from just the 2 id columns.
    //So, we need below class and OnModelCreating code which tells the EF core to consider payloads as well (DateJoined in this case).
    public class BattleSamurai
    {
        public int SamuraiId { get; set; }
        public int BattleId { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
