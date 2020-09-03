using DescLogicFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.IO;

namespace DescDatabase
{
    class Program
    {
        public static IConfigurationRoot configuration;
      
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Build configuration
           configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();


            //IServiceCollection services = new ServiceCollection();
            //services.AddDbContext<DescDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DESCDatabase")));

            Console.WriteLine(configuration.GetConnectionString("DESCDatabase"));

            using (var db = new DescDBContext())
            {
                for (int i = 30; i < 40; i++)
                {
                    var ld = new LithologicDescription() { LithologicID=i.ToString(), DescriptionGroup = "Paleo", DescriptionType = "Macro", DescriptionTab = "Nanno", DescriptionReport = "Paleo"  };
                    db.LithologicDescriptions.AddAsync(ld);
                    db.SaveChangesAsync();
                }
            }
        }

    }
    
}
