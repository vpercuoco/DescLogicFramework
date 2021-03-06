﻿// <auto-generated />
using System;
using DescDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DescDatabase.Migrations
{
    [DbContext(typeof(DescDBContext))]
    [Migration("20200723192428_AddedMeasurementDataList")]
    partial class AddedMeasurementDataList
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DescLogicFramework.ColumnValuePair", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ColumnName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("LithologicDescriptionID")
                        .HasColumnType("int");

                    b.Property<int?>("MeasurementID")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("LithologicDescriptionID");

                    b.HasIndex("MeasurementID");

                    b.ToTable("ColumnValuePair");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicDescription", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescriptionGroup")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DescriptionReport")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DescriptionTab")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DescriptionType")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("LithologicID")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("SectionInfoSectionTextID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("SectionInfoSectionTextID");

                    b.ToTable("LithologicDescriptions");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicSubinterval", b =>
                {
                    b.Property<int>("LithologicSubID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("SectionInfoSectionTextID")
                        .HasColumnType("int");

                    b.HasKey("LithologicSubID");

                    b.HasIndex("SectionInfoSectionTextID");

                    b.ToTable("LithologicSubinterval");
                });

            modelBuilder.Entity("DescLogicFramework.Measurement", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("InstrumentReport")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("InstrumentSystem")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100);

                    b.Property<int?>("LithologicDescriptionID")
                        .HasColumnType("int");

                    b.Property<int?>("LithologicSubintervalLithologicSubID")
                        .HasColumnType("int");

                    b.Property<int?>("SectionInfoSectionTextID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("LithologicDescriptionID");

                    b.HasIndex("LithologicSubintervalLithologicSubID");

                    b.HasIndex("SectionInfoSectionTextID");

                    b.ToTable("MeasurementDescriptions");
                });

            modelBuilder.Entity("DescLogicFramework.SectionInfo", b =>
                {
                    b.Property<int>("SectionTextID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Core")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Expedition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hole")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SampleID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Section")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Site")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SectionTextID");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("DescLogicFramework.ColumnValuePair", b =>
                {
                    b.HasOne("DescLogicFramework.LithologicDescription", null)
                        .WithMany("Data")
                        .HasForeignKey("LithologicDescriptionID");

                    b.HasOne("DescLogicFramework.Measurement", null)
                        .WithMany("Data")
                        .HasForeignKey("MeasurementID");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicDescription", b =>
                {
                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoSectionTextID");
                });

            modelBuilder.Entity("DescLogicFramework.LithologicSubinterval", b =>
                {
                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoSectionTextID");
                });

            modelBuilder.Entity("DescLogicFramework.Measurement", b =>
                {
                    b.HasOne("DescLogicFramework.LithologicDescription", "LithologicDescription")
                        .WithMany()
                        .HasForeignKey("LithologicDescriptionID");

                    b.HasOne("DescLogicFramework.LithologicSubinterval", "LithologicSubinterval")
                        .WithMany()
                        .HasForeignKey("LithologicSubintervalLithologicSubID");

                    b.HasOne("DescLogicFramework.SectionInfo", "SectionInfo")
                        .WithMany()
                        .HasForeignKey("SectionInfoSectionTextID");
                });
#pragma warning restore 612, 618
        }
    }
}
