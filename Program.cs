using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace CodeCamp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create web host
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                // instantiate Startup class
                .UseStartup<Startup>()
                .Build();
            
            // start web host
            host.Run();
        }
    }
}
