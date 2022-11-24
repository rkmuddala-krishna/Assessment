using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
namespace DSO.OTP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseWebRoot("wwwroot");
                    //webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
                    webBuilder.UseKestrel(opts =>
                                    {
                                        // Bind directly to a socket handle or Unix socket
                                        // opts.ListenHandle(123554);
                                        // opts.ListenUnixSocket("/tmp/kestrel-test.sock");
                                        //opts.Listen(IPAddress.Loopback, port: 5001);
                                         opts.ListenAnyIP(5000);
                                        // opts.ListenLocalhost(5004, opts => opts.UseHttps());
                                        // opts.ListenLocalhost(5005, opts => opts.UseHttps());
                                    });
                });


    }
}
