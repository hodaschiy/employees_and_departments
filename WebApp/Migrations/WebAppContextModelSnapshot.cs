﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApp.Data;

#nullable disable

namespace WebApp.Migrations
{
    [DbContext(typeof(WebAppContext))]
    partial class WebAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebApp.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ChiefId")
                        .HasColumnType("integer");

                    b.Property<int?>("Level")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("Tree")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChiefId")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("WebApp.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ChiefId")
                        .HasColumnType("integer");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tree")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChiefId")
                        .IsUnique();

                    b.HasIndex("DepartmentId");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("WebApp.Models.Department", b =>
                {
                    b.HasOne("WebApp.Models.Employee", "Chief")
                        .WithOne()
                        .HasForeignKey("WebApp.Models.Department", "ChiefId");

                    b.HasOne("WebApp.Models.Department", "Parent")
                        .WithMany("ChildDepartments")
                        .HasForeignKey("ParentId");

                    b.Navigation("Chief");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("WebApp.Models.Employee", b =>
                {
                    b.HasOne("WebApp.Models.Employee", "Chief")
                        .WithOne("Subordinate")
                        .HasForeignKey("WebApp.Models.Employee", "ChiefId");

                    b.HasOne("WebApp.Models.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chief");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("WebApp.Models.Department", b =>
                {
                    b.Navigation("ChildDepartments");

                    b.Navigation("Employees");
                });

            modelBuilder.Entity("WebApp.Models.Employee", b =>
                {
                    b.Navigation("Subordinate");
                });
#pragma warning restore 612, 618
        }
    }
}
