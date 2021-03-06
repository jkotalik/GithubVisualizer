﻿// <auto-generated />
using System;
using ConsoleApp9;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GithubVisualizer.Migrations
{
    [DbContext(typeof(CheckContext))]
    [Migration("20190402171006_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ConsoleApp9.Check", b =>
                {
                    b.Property<int>("CheckId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CheckTypeId");

                    b.Property<DateTimeOffset>("Finished");

                    b.Property<string>("PullRequestName");

                    b.Property<string>("SHA");

                    b.Property<DateTimeOffset>("Start");

                    b.Property<double>("TimeTaken");

                    b.HasKey("CheckId");

                    b.HasIndex("CheckTypeId");

                    b.ToTable("Checks");
                });

            modelBuilder.Entity("ConsoleApp9.CheckType", b =>
                {
                    b.Property<int>("CheckTypeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("CheckTypeId");

                    b.ToTable("CheckTypes");
                });

            modelBuilder.Entity("ConsoleApp9.Check", b =>
                {
                    b.HasOne("ConsoleApp9.CheckType", "CheckType")
                        .WithMany("Checks")
                        .HasForeignKey("CheckTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
