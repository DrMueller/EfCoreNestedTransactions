using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mmu.Mlh.ConsoleExtensions.Areas.Commands.Models;
using Mmu.Mlh.ConsoleExtensions.Areas.ConsoleOutput.Services;
using Mmu.Mlh.LanguageExtensions.Areas.Collections;

namespace EfCoreNestedTransactions.ConsoleCommands
{
    public class TestPerformance : IConsoleCommand
    {
        private const int AmountOfEntities = 1000000;

        private readonly IConsoleWriter _consoleWriter;
        public string Description { get; } = "Test Performance";
        public ConsoleKey Key { get; } = ConsoleKey.F3;

        public TestPerformance(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public async Task ExecuteAsync()
        {
            Stopwatch stopwatch;

            await CleanDbAsync();

            await using (var db = new BloggingContext())
            {
                stopwatch = Stopwatch.StartNew();
                Enumerable.Range(0, 1000000).ForEach(_ => db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" }));
                await db.SaveChangesAsync();
                stopwatch.Stop();
                _consoleWriter.WriteLine($"Insert {AmountOfEntities} blogs without custom transaction, 1 SaveChanges: {stopwatch.Elapsed.Seconds} seconds");
            }

            await CleanDbAsync();

            await using (var db = new BloggingContext())
            {
                stopwatch = Stopwatch.StartNew();
                Enumerable.Range(0, 1000000).ForEach(_ => db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" }));
                await db.SaveChangesAsync();
                stopwatch.Stop();
                _consoleWriter.WriteLine($"Insert 100'000 blogs without custom transaction, 1 SaveChanges: {stopwatch.Elapsed.Seconds} seconds");
            }
        }

        private async Task CleanDbAsync()
        {
            await using var db = new BloggingContext();
            db.Blogs.RemoveRange(db.Blogs);
            await db.SaveChangesAsync();
        }
    }
}