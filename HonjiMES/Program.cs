using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HonjiMES
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(
                    context.Configuration.GetSection("Kestrel"));
            }).ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseHttpSys(options =>
                    //{
                    //    options.UrlPrefixes.Add("http://10.10.10.220:5000");
                    //});
                    webBuilder.UseStartup<Startup>();
                });
    }
}
