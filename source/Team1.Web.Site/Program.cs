using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Team1.Web.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) => 
                {
                    logging.AddEventLog(new Microsoft.Extensions.Logging.EventLog.EventLogSettings()
                    {
                        //SourceName = "Team1.Web.Site"
                    });
                })
                .UseStartup<Startup>();
    }
}
