using Lamar;
using Mmu.Mlh.ConsoleExtensions.Areas.Commands.Models;

namespace EfCoreNestedTransactions.Infrastructure.DependencyInjection
{
    public class ServiceRegistryCollection : ServiceRegistry
    {
        public ServiceRegistryCollection()
        {
            Scan(
                scanner =>
                {
                    scanner.AssemblyContainingType<ServiceRegistryCollection>();
                    scanner.AddAllTypesOf<IConsoleCommand>();
                    scanner.WithDefaultConventions();
                });
        }
    }
}