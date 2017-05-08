using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyWifi.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageUploads",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageVersionEntity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageHash = table.Column<string>(nullable: true),
                    ImageUploadEntityId = table.Column<long>(nullable: true),
                    Version = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageVersionEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageVersionEntity_ImageUploads_ImageUploadEntityId",
                        column: x => x.ImageUploadEntityId,
                        principalTable: "ImageUploads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageVersionEntity_ImageUploadEntityId",
                table: "ImageVersionEntity",
                column: "ImageUploadEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageVersionEntity");

            migrationBuilder.DropTable(
                name: "ImageUploads");
        }
    }
}
