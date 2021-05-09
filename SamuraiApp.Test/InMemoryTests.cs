using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Diagnostics;

namespace SamuraiApp.Test
{
    [TestClass]
    public class InMemoryTests
    {
        [TestMethod]
        public void CanInsertSamuraiIntoDatabase()
        {
            //To make the dbcontext use in-memory provider, create option and pass it to constructor of dbcontext
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("CanInsertSamurai");
            using (var context = new SamuraiContext(builder.Options))
            {
                var samurai = new Samurai { Name="Hachiko"};
                context.Samurais.Add(samurai);
                Assert.AreEqual(EntityState.Added, context.Entry(samurai).State);

            }
        }
    }
}
