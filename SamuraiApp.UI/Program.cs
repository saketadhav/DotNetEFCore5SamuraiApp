using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        //private static SamuraiContext _contextNT = new SamuraiContextNoTracking();
        //You can use _contextNT for non tracking queries now.

        static void Main(string[] args)
        {
            //Database.EnsureCreated() : Checks if database is already created. If not, it will create one with help of DBContext.
            //Database.EnsureCreated() is used only for dev or testing purpose.
            //On prod, we will create database from scripts generated from script-migration

            //DBContext => database, DBSet => table

            _context.Database.EnsureCreated();

            #region Simple / Basic queries

            //Simple Queries
            GetSamurais("Before Add:");
            AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
            GetSamurais("After Add:");
            QueryFilters();
            QueryFilterAggregates();
            RetrieveAndUpdate();
            RetrieveAndUpdateMultiple();
            MultipleDatabaseOperations();
            RetrieveAndDelete();

            //Disconnected context
            QueryAndUpdateBattle_Disconnected();

            //Avoid / Disable tracking mechanism
            GetResultsYetAvoidTracking();
            #endregion

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region One to Many relationships

            //Adding related data
            InsertNewSamuraiWithAQuote();
            InsertNewSamuraiWithManyQuotes();
            AddQuoteToExistingSamuraiBeingTracked();
            AddQuoteToExistingSamuraiNotTracked(6);
            Simpler_AddQuoteToExistingSamuraiNotTracked(6);
            AddBattles();

            //Getting / Fetching : Methods to load related data: Eager loading, query projections, explicit loading, lazy loading
            EagerLoadSamuraiWithQuotes();
            ProjectSomeProperties();
            ExplicitLoadQuotes();
            LazyLoadQuotes();

            //Filtering with related data : apply filters using conditions on related data, without loading related data
            FilteringWithRelatedData();

            //Modifying / Updating related data
            ModifyingRelatedDataWhenTracked();
            ModifyingRelatedDataWhenNotTracked();
            #endregion
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            #region Many to many relationships

            AddingNewSamuraiToAnExistingBattle();
            ReturnBattleWithSamurais();
            ReturnAllBattlesWithSamurais();
            AddAllSamuraisToAllBattles();
            RemoveSamuraiFromBattleExplicitly();

            #endregion
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            #region One to one relationships

            AddNewSamuraiWithHorse();
            AddNewHorseToSamuraiUsingId();
            AddNewHorseToSamuraiObject();
            GetSamuraiWithHorse();
            GetHorsesWithSamurai();

            #endregion

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        #region Simple Queries

        /// Simple Queries ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void AddSamuraisByName(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name});
            }
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            //Linq method to fetch data
            var samurais = _context.Samurais.ToList();

            //Linq query syntax to fetch data
            //var samurais = (from s in _context.Samurais select s).ToList();

            Console.WriteLine($"{text} : Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

        private static void QueryFilters()
        {
            //Select * from Samurais where Name = Sampson
            var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();

            //Select * from Samurais where Name like '%Sa%'
            samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "%Sa%")).ToList();
            //or
            samurais = _context.Samurais.Where(s => s.Name.Contains("J")).ToList();
        }

        private static void QueryFilterAggregates()
        {
            //First, FirstOrDefault, Single, SingleOrDefault, Last, LastOrDefault, Cont, Min, Max, Average, Sum

            var samurai = _context.Samurais.Where(s => s.Name == "Sampson").FirstOrDefault();
            //or
            samurai = _context.Samurais.FirstOrDefault(s => s.Name == "Sampson");

            samurai = _context.Samurais.Find(2); //Find is not linq method, it's dbset method and immediately gets result without firing query if object with this key is present.
        }

        private static void RetrieveAndUpdate()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name = "Peter";
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultiple()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            //foreach (var samurai in samurais)
            //{
            //    samurai.Name += "_Updated";
            //}
            samurais.ForEach(x => x.Name += "_Updated");
            _context.SaveChanges();
        }

        private static void MultipleDatabaseOperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "_UpdatedAgain";
            _context.Samurais.Add(new Samurai { Name = "Shino" });
            _context.SaveChanges();
        }

        private static void RetrieveAndDelete()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        #endregion

        #region Disconnected context

        /// Disconnected context ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void QueryAndUpdateBattle_Disconnected()
        {
            List<Battle> disconnectedBattles;
            using (var context1 = new SamuraiContext())
            {
                disconnectedBattles = context1.Battles.ToList();
            } //context1 is disposed

            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 1);
            });

            using (var context2 = new SamuraiContext())
            {
                context2.UpdateRange(disconnectedBattles);
                context2.SaveChanges();
            }
        }

        #endregion

        #region Avoid / Disable tracking mechanism

        /// Avoid / Disable tracking mechanism ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void GetResultsYetAvoidTracking()
        {
            //AsNoTracking : ensures that dbcontext does not create entity entry objects to track results for query
            var samurais = _context.Samurais.AsNoTracking().ToList();

            //No Tracking improves performance 

            //Else, we can use 
            //ChangeTracker.QueryTrackingBehaviour = QueryTrackingBehaviour.NoTracking
            //while creating context, so that the context won't track anything (no query will be tracked by default).
            //You can still explicitly track some query with
            //<dbset>.AsTracking()

            //To swtich on and off tracking in context: (tracking is on by default)
            //Refer SamuraiContextNoTracking()
        }

        #endregion

        #region Adding related data

        /// Adding related data ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kembai Shimada",
                Quotes = new List<Quote>{ new Quote { Text = "I've come to save you"} }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote> { 
                    new Quote { Text = "Watch out for my sharp sword!" },
                    new Quote { Text = "I told you to watch out for my sharp sword! Oh well." }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiBeingTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote { Text = "I bet you're happy that I saved you!" });
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote { Text = "Now that I saved you, will you feed me dinner?" });
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Update(samurai);
                newContext.SaveChanges();
            }
        }

        private static void Simpler_AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var quote = new Quote { Text = "Thanks for the dinner.", SamuraiId = samuraiId };
            using var newContext = new SamuraiContext();
            newContext.Qoutes.Add(quote);
            newContext.SaveChanges();
        }

        private static void AddBattles()
        {
            List<Battle> newBattles = new List<Battle>();
            newBattles.Add(new Battle { Name = "Battle of Shanghai" });
            newBattles.Add(new Battle { Name = "Battle of Kamako" });
            foreach (Battle battle in newBattles)
            {
                _context.Battles.Add(battle);
            }
            _context.SaveChanges();
        }

        #endregion

        #region Methods to load related data

        /// Methods to load related data ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //1. EagerLoading => Include related objects in query
        //2. Query Projections => Define the shape of query results
        //3. Explicit Loading => Loading related data for objects which are already in memory
        //4. Lazy Loading => on-the-fly retrieval of related data


        private static void EagerLoadSamuraiWithQuotes()
        {
            //Includes -> creates a left join query : Samurai left join Quotes
            var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();

            //AsSplitQuery -> query broken up into multiple queries : use inner join
            var splitQuery = _context.Samurais.AsSplitQuery().Include(s => s.Quotes).ToList();

            //To filter or sort (order by) results while querying with eager loading : new fewature of ef core added in 2020
            var filteredInclude1 = _context.Samurais.Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();

            var filteredInclude2 = _context.Samurais.Where(s => s.Name.Contains("Sampson")).Include(s => s.Quotes).FirstOrDefault();
        }

        private static void ProjectSomeProperties()
        {
            //Project to 'Anonymous' type
            var somePropertiesToAnonymousType = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();

            //Project to some known / defined type
            var somePropertiesToDefinedType = _context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();

            //Inclue one more property (quotes) while projecting to 'Anonymous' type
            var somePropertiesWithQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes }).ToList();

            //Select aggregate of related data
            var somePropertiesWithQuotesAggregate = _context.Samurais.Select(s => new { s.Id, s.Name, NumberOfQuotes = s.Quotes.Count }).ToList();

            //Filter related data
            var somePropertiesWithQuotesFiltered = _context.Samurais.Select(s => new { s.Id, s.Name, HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy")) }).ToList();

            //Full entity objects with filtered related data
            var allSamuraisWithQuotesFiltered = _context.Samurais.Select(s => new { Samurai = s, HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy")) }).ToList();

            //In quick watch, you can see the change tracker entries in _context.ChangeTracker.Entries, results
        }

            //Temporary class to project qeury result into this type
            public class IdAndName
            {
                public IdAndName(int id, string name)
                {
                    Id = id;
                    Name = name;
                }

                public int Id { get; set; }
                public string Name { get; set; }
            }

        private static void ExplicitLoadQuotes()
        {
            //With 'samurai' object already in memory
            //_context.Entry(samurai).Collection(s => s.Quotes).Load();  -- DbContext.Entry().Collection().Load()   -- For adding info of collection property
            //_context.Entry(samurai).Reference(s => s.Horse).Load();     -- DbContext.Entry().Reference().Load()   -- For adding info of reference property

            //Make sure there is a horse in DB, then clear the context's change tracker
            _context.Set<Horse>().Add(new Horse { SamuraiId = 1, Name = "Mr. Ed" });
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            //---

            var samurai = _context.Samurais.Find(1);

            _context.Entry(samurai).Collection(s => s.Quotes).Load();

            _context.Entry(samurai).Reference(s => s.Horse).Load();

        }

        private static void LazyLoadQuotes()
        {
            //Lazy loading is turned OFF by default.
            //You have to explicitly mention Lazy Loading to enable it.
            //For enabling lazy loading
                //1. Every navigation property in every entity must be virtual
                //2. Need Microsoft.EntityFramework.Proxies package
                //3. OnConfiguring in dbcontext class : optionsBuilder.UseLazyLoadingProxies()

        }

        #endregion

        #region Filtering with related data : apply filters using conditions on related data, without loading related data

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais.Where(s => s.Quotes.Any(q => q.Text.Contains("happy"))).ToList();
        }

        #endregion

        #region Modifying related data

        /// Modifying related data ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = "Did you hear that?";
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text += "Did you hear that again?";

            using var newContext = new SamuraiContext();
            //newContext.Qoutes.Update(quote);
            //or
            newContext.Entry(quote).State = EntityState.Modified;
            newContext.SaveChanges();
        }

        #endregion

        #region Many to many relationships

        /// Many to many relationships ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void AddingNewSamuraiToAnExistingBattle()
        {
            var battle = _context.Battles.FirstOrDefault();
            battle = battle == null ? new Battle() : battle;
            battle.Samurais.Add(new Samurai { Name = "Takeda Shingen" });
            _context.SaveChanges();
        }

        private static void ReturnBattleWithSamurais()
        {
            var battle = _context.Battles.Include(b => b.Samurais).FirstOrDefault();
        }

        private static void ReturnAllBattlesWithSamurais()
        {
            var battle = _context.Battles.Include(b => b.Samurais).ToList();
        }

        private static void AddAllSamuraisToAllBattles()
        {
            //To avoid sql exception of violation of foreign key, clear data from BattleSamurai if any

            var allbattles = _context.Battles.ToList();
            var allsamurais = _context.Samurais.ToList();
            foreach (var battle in allbattles)
            {
                battle.Samurais.AddRange(allsamurais);
            }
            _context.SaveChanges();
        }

        private static void RemoveSamuraiFromBattleExplicitly()
        {
            var b_s = _context.Set<BattleSamurai>().SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 6);
            if (b_s != null)
            {
                _context.Remove(b_s);
                _context.SaveChanges();
            }
        }

        #endregion

        #region One to One relationships

        /// One to One relationships ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiUsingId()
        {
            var horse = new Horse { Name = "Scout", SamuraiId = 6 };
            _context.Add(horse);
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(7);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();
        }

        private static void GetSamuraiWithHorse()
        {
            var samurais = _context.Samurais.Include(s => s.Horse).ToList();
        }

        private static void GetHorsesWithSamurai()
        {
            var horseOnly = _context.Set<Horse>().Find(2);

            var horseWithSamurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Horse.Id == 3);

            var horseSamuraiPairs = _context.Samurais
                                    .Where(s => s.Horse != null)
                                    .Select(s => new { Horse = s.Horse, Samurai = s })
                                    .ToList();
        }


        #endregion

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
