﻿// <auto-generated />
using System;
using DescLogicFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DescLogicFramework.Migrations
{
    [DbContext(typeof(DescDBContext))]
    partial class DescDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DescLogicFramework.DescriptionColumnValuePair", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ColumnName")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("CorrectedColumnName")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("CorrectedValue")
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.Property<int?>("LithologicDescriptionID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.HasKey("ID");

                    b.HasIndex("LithologicDescriptionID");

                    b.ToTable("DescriptionColumnValuePairs");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicDescription", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescriptionReport")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<double?>("EndOffset")
                        .HasColumnType("float");

                    b.Property<int?>("SectionInfoID")
                        .HasColumnType("int");

                    b.Property<double?>("StartOffset")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.HasIndex("SectionInfoID");

                    b.ToTable("LithologicDescriptions");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicSubinterval", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("EndOffset")
                        .HasColumnType("float");

                    b.Property<int?>("LithologicDescriptionID")
                        .HasColumnType("int");

                    b.Property<int?>("LithologicSubID")
                        .HasColumnType("int");

                    b.Property<int?>("SectionInfoID")
                        .HasColumnType("int");

                    b.Property<double?>("StartOffset")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.HasIndex("LithologicDescriptionID");

                    b.HasIndex("SectionInfoID");

                    b.ToTable("LithologicSubintervals");
                });

            modelBuilder.Entity("DescLogicFramework.Measurement", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double?>("EndOffset")
                        .HasColumnType("float");

                    b.Property<string>("InstrumentReport")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("InstrumentSystem")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int?>("SectionInfoID")
                        .HasColumnType("int");

                    b.Property<double?>("StartOffset")
                        .HasColumnType("float");

                    b.Property<string>("TestNumber")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("TextID")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("ID");

                    b.HasIndex("SectionInfoID");

                    b.ToTable("MeasurementDescriptions");
                });

            modelBuilder.Entity("DescLogicFramework.MeasurementColumnValuePair", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ColumnName")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("LithologicID")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("LithologicSubID")
                        .HasColumnType("int");

                    b.Property<int?>("MeasurementID")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasMaxLength(5000)
                        .HasColumnType("varchar(5000)");

                    b.HasKey("ID");

                    b.HasIndex("MeasurementID");

                    b.ToTable("MeasurementColumnValuePairs");
                });

            modelBuilder.Entity("DescLogicFramework.SectionInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArchiveTextID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Core")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Expedition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Half")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentTextID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SampleID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Section")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Site")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkingTextID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("LithologicSubintervalMeasurement", b =>
                {
                    b.Property<int>("LithologicSubintervalsID")
                        .HasColumnType("int");

                    b.Property<int>("MeasurementsID")
                        .HasColumnType("int");

                    b.HasKey("LithologicSubintervalsID", "MeasurementsID");

                    b.HasIndex("MeasurementsID");

                    b.ToTable("LithologicSubintervalMeasurement");
                });

            modelBuilder.Entity("DescLogicFramework.DescriptionColumnValuePair", b =>
                {
                    b.HasOne("DescLogicFramework.LithologicDescription", null)
                        .WithMany("DescriptionColumnValues")
                        .HasForeignKey("LithologicDescriptionID");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicDescription", b =>
                {
                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoID");

                    b.Navigation("SectionInfo");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicSubinterval", b =>
                {
                    b.HasOne("DescLogicFramework.LithologicDescription", "LithologicDescription")
                        .WithMany("LithologicSubintervals")
                        .HasForeignKey("LithologicDescriptionID");

                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoID");

                    b.Navigation("LithologicDescription");

                    b.Navigation("SectionInfo");
                });

            modelBuilder.Entity("DescLogicFramework.Measurement", b =>
                {
                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoID");

                    b.Navigation("SectionInfo");
                });

            modelBuilder.Entity("DescLogicFramework.MeasurementColumnValuePair", b =>
                {
                    b.HasOne("DescLogicFramework.Measurement", null)
                        .WithMany("MeasurementData")
                        .HasForeignKey("MeasurementID");
                });

            modelBuilder.Entity("LithologicSubintervalMeasurement", b =>
                {
                    b.HasOne("DescLogicFramework.LithologicSubinterval", null)
                        .WithMany()
                        .HasForeignKey("LithologicSubintervalsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DescLogicFramework.Measurement", null)
                        .WithMany()
                        .HasForeignKey("MeasurementsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DescLogicFramework.LithologicDescription", b =>
                {
                    b.Navigation("DescriptionColumnValues");

                    b.Navigation("LithologicSubintervals");
                });

            modelBuilder.Entity("DescLogicFramework.Measurement", b =>
                {
                    b.Navigation("MeasurementData");
                });
#pragma warning restore 612, 618
        }
    }
}
