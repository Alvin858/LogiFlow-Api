using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogiFlow.Infrastructure.Migrations;

[Migration("20260911100000_AddBillingNotifications")]
public partial class AddBillingNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Invoices",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                ShipmentId = table.Column<int>(type: "int", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                IssuedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Invoices", x => x.Id);
                table.ForeignKey("FK_Invoices_Customers_CustomerId", x => x.CustomerId, "Customers", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Invoices_Shipments_ShipmentId", x => x.ShipmentId, "Shipments", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                IsRead = table.Column<bool>(type: "bit", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
                table.ForeignKey("FK_Notifications_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                InvoiceId = table.Column<int>(type: "int", nullable: false),
                PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                PaidAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey("FK_Payments_Invoices_InvoiceId", x => x.InvoiceId, "Invoices", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Invoices_InvoiceNumber", table: "Invoices", column: "InvoiceNumber", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Invoices_ShipmentId", table: "Invoices", column: "ShipmentId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Invoices_CustomerId", table: "Invoices", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_Notifications_UserId_IsRead_CreatedAtUtc", table: "Notifications", columns: new[] { "UserId", "IsRead", "CreatedAtUtc" });
        migrationBuilder.CreateIndex(name: "IX_Payments_PaymentReference", table: "Payments", column: "PaymentReference", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Payments_InvoiceId", table: "Payments", column: "InvoiceId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Notifications");
        migrationBuilder.DropTable(name: "Payments");
        migrationBuilder.DropTable(name: "Invoices");
    }
}
