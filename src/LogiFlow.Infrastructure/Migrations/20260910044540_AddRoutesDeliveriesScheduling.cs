using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogiFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutesDeliveriesScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartLocation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    StartTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProofsOfDelivery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignaturePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProofsOfDelivery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProofsOfDelivery_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteStops_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DriverId",
                table: "Deliveries",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_RouteId",
                table: "Deliveries",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_ShipmentId",
                table: "Deliveries",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_Status",
                table: "Deliveries",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_VehicleId",
                table: "Deliveries",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProofsOfDelivery_DeliveryId",
                table: "ProofsOfDelivery",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_Status",
                table: "Routes",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_RouteId_StopOrder",
                table: "RouteStops",
                columns: new[] { "RouteId", "StopOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DriverId",
                table: "Schedules",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EndTimeUtc",
                table: "Schedules",
                column: "EndTimeUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ShipmentId",
                table: "Schedules",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_StartTimeUtc",
                table: "Schedules",
                column: "StartTimeUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Status",
                table: "Schedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_VehicleId",
                table: "Schedules",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProofsOfDelivery");

            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
