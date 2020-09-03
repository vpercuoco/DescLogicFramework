using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{
        class DescDBContext : DbContext
        {


            //I am manually overriding this method so I can configure the DBcontext options, see definiition of DBContext or MIcrosoft Documentation for more info
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectsV13;Database=DESCDatabase;Trusted_Connection=True;");

            }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LithologicDescription>()
                .OwnsOne(c => c.SectionInfo)
                .ToTable("Sections");

            modelBuilder.Entity<LithologicSubinterval>()
                  .Ignore(c => c.SectionInfo);

            modelBuilder.Entity<Measurement>()
             .Ignore(c => c.SectionInfo);

            // EntityTypeBuilder.Ignore
        }  

       
        public DbSet<LithologicDescription> LithologicDescriptions { get; set; }

            public DbSet<Measurement> MeasurementDescriptions { get; set; }

        //    public DbSet<SectionInfo> Sections { get; set; }

            public DbSet<DescriptionColumnValuePair> DescriptionColumnValuePairs { get; set; }

            public DbSet<MeasurementColumnValuePair> MeasurementColumnValuePairs { get; set; }

            public DbSet<LithologicSubinterval> LithologicSubintervals { get; set; }

            //public object LithologicDescription { get; internal set; }


        }
}
