using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.LogLevel;

namespace HeadlessArchitect.Website
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(typeof(Program) + ".Main() : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
            IHost host = CreateHostBuilder(args).Build();
            Console.WriteLine(typeof(Program) + ".Build() : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
            host.Run();
            Console.WriteLine(typeof(Program) + ".Run() : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    Console.WriteLine(typeof(Program) + ".AddConsole() : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    Console.WriteLine(typeof(Program) + ".ConfigureWebHostDefaults() : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
                });
    }
}
