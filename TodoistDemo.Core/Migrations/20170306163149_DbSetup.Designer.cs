using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoistDemo.Core.Storage.Database;

namespace TodoistDemo.Core.Migrations
{
    [DbContext(typeof(TodoistContext))]
    [Migration("20170306163149_DbSetup")]
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

                    b.Property<int?>("AssignedBy");

                    b.Property<int>("Checked");

                    b.Property<int>("Collapsed");

                    b.Property<string>("Content");

                    b.Property<string>("DateAdded");

                    b.Property<string>("DateLanguage");

                    b.Property<int>("DayOrder");

                    b.Property<string>("FormattedDate");

                    b.Property<int>("InHistory");

                    b.Property<int>("Indent");

                    b.Property<int>("IsArchived");

                    b.Property<int>("IsDeleted");

                    b.Property<int>("ItemOrder");

                    b.Property<int>("Priority");

                    b.Property<int>("ProjectId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("TodoistDemo.Core.Storage.Database.DbUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Avatar");

                    b.Property<string>("AvatarBig");

                    b.Property<string>("AvatarMedium");

                    b.Property<string>("AvatarSmall");

                    b.Property<int>("CompletedCount");

                    b.Property<string>("Email");

                    b.Property<string>("FullName");

                    b.Property<int>("SortOrder");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
        }
    }
}
