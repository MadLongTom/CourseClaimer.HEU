﻿// <auto-generated />
using System;
using CourseClaimer.Wisedu.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CourseClaimer.HEU.Shared.Migrations
{
    [DbContext(typeof(ClaimDbContext))]
    [Migration("20240722051857_AddQuartz")]
    partial class AddQuartz
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("CourseClaimer.Wisedu.Shared.Models.Database.ClaimRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ClaimTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Course")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ClaimRecords");
                });

            modelBuilder.Entity("CourseClaimer.Wisedu.Shared.Models.Database.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Categories")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Course")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Priority")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("CourseClaimer.Wisedu.Shared.Models.Database.EntityRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EntityRecords");
                });

            modelBuilder.Entity("CourseClaimer.Wisedu.Shared.Models.Database.JobRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExecuteTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("JobRecords");
                });
#pragma warning restore 612, 618
        }
    }
}