using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bifrost.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPortals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortalDefinitions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreationUser = table.Column<string>(type: "TEXT", nullable: false),
                    MaxInstanceCount = table.Column<int>(type: "INTEGER", nullable: false),
                    VpnType = table.Column<string>(type: "TEXT", nullable: false),
                    VpnConfig = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalInstances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    PortalId = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalInstances_PortalDefinitions_PortalId",
                        column: x => x.PortalId,
                        principalTable: "PortalDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortalHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreationUser = table.Column<string>(type: "TEXT", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalHistory_PortalInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "PortalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortMappings",
                columns: table => new
                {
                    MappedPort = table.Column<string>(type: "TEXT", nullable: false),
                    InstanceId = table.Column<string>(type: "TEXT", nullable: false),
                    Service = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortMappings", x => new { x.InstanceId, x.MappedPort });
                    table.ForeignKey(
                        name: "FK_PortMappings_PortalInstances_InstanceId",
                        column: x => x.InstanceId,
                        principalTable: "PortalInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "PortalNameIndex",
                table: "PortalDefinitions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortalHistory_InstanceId",
                table: "PortalHistory",
                column: "InstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalInstances_PortalId",
                table: "PortalInstances",
                column: "PortalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortalHistory");

            migrationBuilder.DropTable(
                name: "PortMappings");

            migrationBuilder.DropTable(
                name: "PortalInstances");

            migrationBuilder.DropTable(
                name: "PortalDefinitions");
        }
    }
}
