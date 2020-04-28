using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Deliver.Web
{
    public class Program
    {
#if !DEBUG
        static IConfiguration Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false).Build();
#endif

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if !DEBUG
        .UseKestrel(options =>
            {
                options.Listen(System.Net.IPAddress.Any, 5000);  // http:localhost:5000
                options.Listen(System.Net.IPAddress.Any, 50001, listenOptions =>
                {
                    listenOptions.UseHttps(Config["Cert:Pfx"], Config["Cert:Password"]);
                });
            }) 
#endif
            .ConfigureLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
                .UseStartup<Startup>();
    }
}
