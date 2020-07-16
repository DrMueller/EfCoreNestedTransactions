using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mmu.Mlh.ConsoleExtensions.Areas.Commands.Models;
using Mmu.Mlh.ConsoleExtensions.Areas.ConsoleOutput.Services;

namespace EfCoreNestedTransactions.ConsoleCommands
{
    public class CreateWithTransactionNoSaveChanges : IConsoleCommand
    {
        private readonly IConsoleWriter _consoleWriter;

        public CreateWithTransactionNoSaveChanges(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public async Task ExecuteAsync()
        {
            await using var db = new BloggingContext();

            db.Blogs.RemoveRange(db.Blogs);
            await db.SaveChangesAsync();
            
            await using var transaction = await db.Database.BeginTransactionAsync();
            _consoleWriter.WriteLine($"Transaction ID before: {transaction.TransactionId}");

            db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            //await db.SaveChangesAsync();

            await transaction.CommitAsync();
            //await transaction.RollbackAsync();

            _consoleWriter.WriteLine($"Transaction ID after: {transaction.TransactionId}");
            _consoleWriter.WriteLine($"Blogs persisted: {db.Blogs.Count()}");
        }

        public string Description { get; } = "Transaction without SaveChanges";
        public ConsoleKey Key { get; } = ConsoleKey.F2;
    }
}
