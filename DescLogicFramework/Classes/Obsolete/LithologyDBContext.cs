using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DescLogicFramework
{
    public class LithologyDBContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectsV13;Database=LithologyDatabase;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SectionInfo>(entity => entity.Property(e => e.ID).ValueGeneratedOnAdd());
            modelBuilder.Entity<DescriptionColumnValuePair>(entity => entity.Property(e => e.ID).ValueGeneratedOnAdd());
            modelBuilder.Entity<DescriptionColumnValuePair>(entity => entity.Property(e => e.Value).HasMaxLength(5000));
               
        }

        public DbSet<LithologicDescription> LithologicDescriptions { get; set; }

        public DbSet<SectionInfo> Sections { get; set; }

        public DbSet<DescriptionColumnValuePair> DescriptionColumnValuePairs { get; set; }

        public DbSet<LithologicSubinterval> LithologicSubintervals { get; set; }


    }
}
