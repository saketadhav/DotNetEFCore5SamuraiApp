using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            //Database.EnsureCreated() : Checks if database is already created. If not, it will create one with help of DBContext.
            //Database.EnsureCreated() is used only for dev or testing purpose.
            //On prod, we will create database from scripts generated from script-migration

            //DBContext => database, DBSet => table

            _context.Database.EnsureCreated();
            GetSamurais("Before Add:");
            AddSamurais("Julie", "Sampson");
            GetSamurais("After Add:");

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void AddSamurais(params string[] names)
        {
            foreach (string name in names)
            {
                _context.Samurais.Add(new Samurai { Name = name});
            }
            _context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text} : Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }

    }
}
