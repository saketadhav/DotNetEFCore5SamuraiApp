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

        private static SamuraiContext _contextNT = new SamuraiContextNoTracking();
        //You can use _contextNT for non tracking queries now.

        static void Main(string[] args)
        {
            //Database.EnsureCreated() : Checks if database is already created. If not, it will create one with help of DBContext.
            //Database.EnsureCreated() is used only for dev or testing purpose.
            //On prod, we will create database from scripts generated from script-migration

            //DBContext => database, DBSet => table

            _context.Database.EnsureCreated();
            GetSamurais("Before Add:");
            AddSamuraisByName("Shimada", "Okamoto", "Kikuchio", "Hayashida");
            GetSamurais("After Add:");
            QueryFilters();
            QueryFilterAggregates();
            RetrieveAndUpdate();
            RetrieveAndUpdateMultiple();
            MultipleDatabaseOperations();
            RetrieveAndDelete();

            QueryAndUpdateBattle_Disconnected();

            GetResultsYetAvoidTracking();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

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


    }
}
