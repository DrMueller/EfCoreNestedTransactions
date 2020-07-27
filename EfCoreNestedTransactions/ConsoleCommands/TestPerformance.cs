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
        private const int AmountOfEntities = 100000;
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
                Enumerable.Range(0, AmountOfEntities).ForEach(_ => db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" }));
                await db.SaveChangesAsync();
                stopwatch.Stop();
                _consoleWriter.WriteLine($"Inserting {AmountOfEntities} blogs without custom transaction, 1 SaveChanges: {stopwatch.Elapsed.Seconds} seconds");
            }

            await CleanDbAsync();
            await using (var db = new BloggingContext())
            {
                stopwatch = Stopwatch.StartNew();
                await using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    Enumerable.Range(0, AmountOfEntities).ForEach(_ => db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" }));
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                stopwatch.Stop();
                _consoleWriter.WriteLine($"Insert 100'000 blogs with custom transaction, 1 SaveChanges: {stopwatch.Elapsed.Seconds} seconds");
            }

            // This never returns, as it is too slow

            ////await CleanDbAsync();
            ////await using (var db = new BloggingContext())
            ////{
            ////    stopwatch = Stopwatch.StartNew();

            ////    for (var i = 0; i <= AmountOfEntities; i++)
            ////    {
            ////        db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            ////        await db.SaveChangesAsync();
            ////    }

            ////    stopwatch.Stop();
            ////    _consoleWriter.WriteLine($"Insert 100'000 blogs without custom transaction, 1 SaveChanges per entry: {stopwatch.Elapsed.Seconds} seconds");
            ////}

            // This also never returns, as it is too slow

            ////await CleanDbAsync();
            ////await using (var db = new BloggingContext())
            ////{
            ////    stopwatch = Stopwatch.StartNew();
            ////    await using (var transaction = await db.Database.BeginTransactionAsync())
            ////    {
            ////        for (var i = 0; i <= AmountOfEntities; i++)
            ////        {
            ////            db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            ////            await db.SaveChangesAsync();
            ////        }

            ////        await transaction.CommitAsync();
            ////    }

            ////    stopwatch.Stop();
            ////    _consoleWriter.WriteLine($"Insert 100'000 blogs with custom transaction, 1 SaveChanges per entry: {stopwatch.Elapsed.Seconds} seconds");
            ////}
        }

        private async Task CleanDbAsync()
        {
            await using var db = new BloggingContext();
            db.Blogs.RemoveRange(db.Blogs);
            await db.SaveChangesAsync();
        }
    }
}