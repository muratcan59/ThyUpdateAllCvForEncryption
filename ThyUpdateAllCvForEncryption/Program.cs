using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Owin.Hosting;
using System;

namespace ThyUpdateAllCvForEncryption
{
    internal class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

        }

    }
}
