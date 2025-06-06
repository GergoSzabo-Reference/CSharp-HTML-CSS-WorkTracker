﻿// <auto-generated />
using Autoszerelo_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Autoszerelo_API.Migrations
{
    [DbContext(typeof(AutoszereloDbContext))]
    [Migration("20250525215914_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Autoszerelo.Shared.Ugyfel", b =>
                {
                    b.Property<int>("UgyfelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UgyfelId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Lakcim")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Nev")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UgyfelId");

                    b.ToTable("Ugyfelek");
                });

            modelBuilder.Entity("Autoszerelo_Shared.Munka", b =>
                {
                    b.Property<int>("MunkaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MunkaId"));

                    b.Property<int>("Allapot")
                        .HasColumnType("int");

                    b.Property<int>("GyartasiEv")
                        .HasColumnType("int");

                    b.Property<string>("HibaLeiras")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("HibaSulyossaga")
                        .HasColumnType("int");

                    b.Property<int>("Kategoria")
                        .HasColumnType("int");

                    b.Property<string>("Rendszam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UgyfelId")
                        .HasColumnType("int");

                    b.HasKey("MunkaId");

                    b.ToTable("Munkak");
                });
#pragma warning restore 612, 618
        }
    }
}
