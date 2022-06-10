﻿// <auto-generated />
using System;
using System.Collections.Generic;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AmazonPriceTrackerAPI.Persistence.Migrations
{
    [DbContext(typeof(AmazonPriceTrackerDbContext))]
    [Migration("20220609223353_mig_6")]
    partial class mig_6
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AmazonPriceTrackerAPI.Domain.Entities.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("MailAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("AmazonPriceTrackerAPI.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double?>("CurrentPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Rate")
                        .HasColumnType("double precision");

                    b.Property<string>("StockState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<string>>("TechnicalDetails")
                        .HasColumnType("text[]");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("isTracking")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("AmazonPriceTrackerAPI.Domain.Entities.TrackedProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("CurrentPrice")
                        .HasColumnType("double precision");

                    b.Property<int>("Interval")
                        .HasColumnType("integer");

                    b.Property<DateTime>("MailSendingDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("NextRunTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double>("PriceChange")
                        .HasColumnType("double precision");

                    b.Property<string[]>("PriceHistory")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<double>("TargetPrice")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("TrackedProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
