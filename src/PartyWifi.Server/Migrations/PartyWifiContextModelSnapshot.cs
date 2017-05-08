using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PartyWifi.Server.DataModel;

namespace PartyWifi.Server.Migrations
{
    [DbContext(typeof(PartyWifiContext))]
    partial class PartyWifiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("PartyWifi.Server.DataModel.ImageUploadEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Identifier");

                    b.Property<bool>("IsApproved");

                    b.Property<long>("Size");

                    b.Property<DateTime>("UploadDate");

                    b.HasKey("Id");

                    b.ToTable("ImageUploads");
                });

            modelBuilder.Entity("PartyWifi.Server.DataModel.ImageVersionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageHash");

                    b.Property<long?>("ImageUploadEntityId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("ImageUploadEntityId");

                    b.ToTable("ImageVersionEntity");
                });

            modelBuilder.Entity("PartyWifi.Server.DataModel.ImageVersionEntity", b =>
                {
                    b.HasOne("PartyWifi.Server.DataModel.ImageUploadEntity")
                        .WithMany("Versions")
                        .HasForeignKey("ImageUploadEntityId");
                });
        }
    }
}
