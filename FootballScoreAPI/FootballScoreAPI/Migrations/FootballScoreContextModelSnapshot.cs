﻿// <auto-generated />
using System;
using FootballScoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FootballScoreAPI.Migrations
{
    [DbContext(typeof(FootballScoreContext))]
    partial class FootballScoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FootballScoreAPI.Models.Fixture", b =>
                {
                    b.Property<int>("FixtureId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AwayScore");

                    b.Property<string>("AwayTeam");

                    b.Property<string>("Competition");

                    b.Property<DateTime>("Date");

                    b.Property<int>("HomeScore");

                    b.Property<string>("HomeTeam");

                    b.HasKey("FixtureId");

                    b.ToTable("Fixtures");
                });

            modelBuilder.Entity("FootballScoreAPI.Models.Goal", b =>
                {
                    b.Property<int>("GoalId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FixtureId");

                    b.Property<string>("For");

                    b.Property<int>("Minute");

                    b.Property<bool>("OwnGoal");

                    b.Property<string>("Scorer");

                    b.HasKey("GoalId");

                    b.HasIndex("FixtureId");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("FootballScoreAPI.Models.Goal", b =>
                {
                    b.HasOne("FootballScoreAPI.Models.Fixture", "Fixture")
                        .WithMany("Goals")
                        .HasForeignKey("FixtureId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
