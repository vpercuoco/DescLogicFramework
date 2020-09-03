using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DescLogicFramework
{
    class DescDBContext : DbContext
    {
        

        //I am manually overriding this method so I can configure the DBcontext options, see definiition of DBContext or MIcrosoft Documentation for more info
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectsV13;Database=DESCDatabase;Trusted_Connection=True;");
        }

        public DbSet<LithologicDescription> LithologicDescriptions { get; set; }

        public DbSet<Measurement> MeasurementDescriptions { get; set; }

        public DbSet<SectionInfo> Sections { get; set; }

        public DbSet<ColumnValuePair> ColumnValuePairs { get; set; }

        public DbSet<LithologicSubinterval> LithologicSubintervals { get; set; }

        //public object LithologicDescription { get; internal set; }
    }
}
