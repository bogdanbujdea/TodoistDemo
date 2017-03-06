using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.Core.Migrations
{
    [DbContext(typeof(TodoistContext))]
    [Migration("20170306201857_DbSetup")]
    partial class DbSetup
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("TodoistDemo.Core.Storage.Database.DbItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Checked");

                    b.Property<string>("Content");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("TodoistDemo.Core.Storage.Database.DbUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarBig");

                    b.Property<string>("FullName");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
        }
    }
}
