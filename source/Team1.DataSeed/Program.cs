using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Team1.DataSeed.Seeders;
using Team1.Entities;
using Team1.Infrastructure.UserIdentity;
using System;
using System.Threading.Tasks;

namespace Team1.DataSeed
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Seeder Started...");
            System.Console.WriteLine("Type: ");
            System.Console.WriteLine(" 1 - \"enum\" to seed enum values.");
            System.Console.WriteLine(" 2 - \"addr\" to seed address data.");
            System.Console.WriteLine(" 3 - \"rcu\" to seed roles, companies and users data.");
            System.Console.WriteLine(" 4 - to seed admin data.");

            var builder = new DbContextOptionsBuilder<DataContext>();

            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configBuilder = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            builder.UseSqlServer(configBuilder.GetConnectionString("DefaultConnection"));

            var ups = new UserPermissionService("System");
            ups.Setup(new UserClaimBuilderTester());
            var context = new DataContext(ups, builder.Options, null);

            SeedBase seeder = null;
            while (true)
            {
                var command = System.Console.ReadLine();
                
                switch (command.ToLower())
                {
                    case "1":
                    case "enum":
                        seeder = new SeedEnums(context);
                        Task.Run(async () => await seeder.Seed()).Wait();
                        System.Console.WriteLine("Seed complete");
                        break;
                    case "2":
                    case "addr":
                        seeder = new SeedAddressData(context);
                        Task.Run(async () => await seeder.Seed()).Wait();
                        System.Console.WriteLine("Seed complete");
                        break;
                    case "3":
                    case "rcu":
                        seeder = new SeedRolesCompaniesUsers(context);
                        Task.Run(async () => await seeder.Seed()).Wait();
                        System.Console.WriteLine("Seed complete");
                        break;
                    case "4":
                        seeder = new SeedAdminData(context);
                        Task.Run(async () => await seeder.Seed()).Wait();
                        System.Console.WriteLine("Seed complete");
                        break;
                    default:
                        System.Console.WriteLine("bye");
                        return;
                }
            }
        }
    }
}
