﻿// <auto-generated />
using System;
using CourseClaimer.HEU.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CourseClaimer.HEU.Shared.Migrations
{
    [DbContext(typeof(ClaimDbContext))]
    [Migration("20240717042500_EntityRecord")]
    partial class EntityRecord
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("CourseClaimer.HEU.Shared.Models.Database.ClaimRecord", b =>
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

            modelBuilder.Entity("CourseClaimer.HEU.Shared.Models.Database.Customer", b =>
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

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("CourseClaimer.HEU.Shared.Models.Database.EntityRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EntityRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
