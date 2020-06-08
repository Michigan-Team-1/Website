using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Team1.Infrastructure.UserIdentity;
using System;

namespace Team1.Entities
{
    /// <summary>
    /// Only needed for doing migrations from within the entities project
    /// </summary>
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            //if (!System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Launch();
            var builder = new DbContextOptionsBuilder<DataContext>();

            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configBuilder = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            builder.UseSqlServer(configBuilder.GetConnectionString("DefaultConnection"));

            var ups = new UserPermissionService("System");
            ups.Setup(new UserClaimModel());
            return new DataContext(ups, builder.Options, null);
        }
    }
}
