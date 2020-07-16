using System;
using System.Linq;
using System.Threading.Tasks;
using Mmu.Mlh.ConsoleExtensions.Areas.Commands.Models;
using Mmu.Mlh.ConsoleExtensions.Areas.ConsoleOutput.Services;

namespace EfCoreNestedTransactions.ConsoleCommands
{
    public class CreateWithDefaultTransaction : IConsoleCommand
    {
        private readonly IConsoleWriter _consoleWriter;
        public string Description { get; } = "Create with default transaction";
        public ConsoleKey Key { get; } = ConsoleKey.F1;

        public CreateWithDefaultTransaction(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public async Task ExecuteAsync()
        {
            await using var db = new BloggingContext();

            db.Blogs.RemoveRange(db.Blogs);
            await db.SaveChangesAsync();

            _consoleWriter.WriteLine($"Transaction ID before: {db.Database.CurrentTransaction?.TransactionId.ToString() ?? "No transaction"}");

            db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });

            _consoleWriter.WriteLine($"Transaction ID after: {db.Database.CurrentTransaction?.TransactionId.ToString() ?? "No transaction"}");
            _consoleWriter.WriteLine($"Blogs persisted: {db.Blogs.Count()}");
        }
    }
}