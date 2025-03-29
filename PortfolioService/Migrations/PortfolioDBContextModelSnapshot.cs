﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PortfolioService.Data;

#nullable disable

namespace PortfolioService.Migrations
{
    [DbContext(typeof(PortfolioDBContext))]
    partial class PortfolioDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PortfolioService.Data.Entities.Portfolio", b =>
                {
                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("Ticker")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.ToTable("Portfolios");
                });
#pragma warning restore 612, 618
        }
    }
}
