﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Project.Models;
using System;

namespace Project.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20180613123849_DB")]
    partial class DB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("Project.Models.Authorization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("Password");

                    b.Property<string>("Role");

                    b.HasKey("Id");

                    b.ToTable("Authorizations");
                });

            modelBuilder.Entity("Project.Models.Characteristic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Characteristics");
                });

            modelBuilder.Entity("Project.Models.Curator", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("SId");

                    b.HasKey("Id", "SId");

                    b.HasIndex("SId");

                    b.ToTable("Curators");
                });

            modelBuilder.Entity("Project.Models.DataLink", b =>
                {
                    b.Property<int>("NId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<string>("Link");

                    b.Property<int>("SId");

                    b.HasKey("NId");

                    b.HasIndex("SId");

                    b.ToTable("DataLinks");
                });

            modelBuilder.Entity("Project.Models.Grade", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("SId");

                    b.Property<int>("Level");

                    b.Property<int>("NId");

                    b.HasKey("Id", "SId");

                    b.HasIndex("SId");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("Project.Models.GradeName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("GradeNames");
                });

            modelBuilder.Entity("Project.Models.Technology", b =>
                {
                    b.Property<int>("SId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("SId");

                    b.ToTable("Technologies");
                });

            modelBuilder.Entity("Project.Models.TechnologyCharacteristic", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("SId");

                    b.Property<string>("Description");

                    b.HasKey("Id", "SId");

                    b.HasIndex("SId");

                    b.ToTable("TechnologyCharacteristics");
                });

            modelBuilder.Entity("Project.Models.TestLink", b =>
                {
                    b.Property<int>("NId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<string>("Link");

                    b.Property<int>("SId");

                    b.HasKey("NId");

                    b.HasIndex("SId");

                    b.ToTable("TestLinks");
                });

            modelBuilder.Entity("Project.Models.User", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Email");

                    b.Property<string>("Information");

                    b.Property<string>("Name");

                    b.Property<byte[]>("Photo");

                    b.Property<string>("Surname");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Project.Models.UserTechnology", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("SId");

                    b.Property<int>("Level");

                    b.HasKey("Id", "SId");

                    b.HasIndex("SId");

                    b.ToTable("UserTechnologies");
                });

            modelBuilder.Entity("Project.Models.Curator", b =>
                {
                    b.HasOne("Project.Models.User", "User")
                        .WithMany("Curators")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("Curators")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.DataLink", b =>
                {
                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("DataLinks")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.Grade", b =>
                {
                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("Grades")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.TechnologyCharacteristic", b =>
                {
                    b.HasOne("Project.Models.Characteristic", "Characteristic")
                        .WithMany("TechnologyCharacteristics")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("TechnologyCharacteristics")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.TestLink", b =>
                {
                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("TestLinks")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.User", b =>
                {
                    b.HasOne("Project.Models.Authorization", "Authorization")
                        .WithOne("User")
                        .HasForeignKey("Project.Models.User", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Project.Models.UserTechnology", b =>
                {
                    b.HasOne("Project.Models.User", "User")
                        .WithMany("UserTechnologies")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Project.Models.Technology", "Technology")
                        .WithMany("UserTechnologies")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
