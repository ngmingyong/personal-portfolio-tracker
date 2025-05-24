using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PersonalPortfolioTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "capital_change_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_capital_change_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    last_updated = table.Column<DateOnly>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaction_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_type", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    position_opened_date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    position_closed_date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    is_position_closed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    no_of_shares_held = table.Column<int>(type: "int", nullable: false),
                    total_purchase_cost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    total_dividends_received = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    total_net_sales_proceeds = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    stock_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_position_stock_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stock",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "capital_change",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    entitlement_date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    change_in_no_of_shares = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    capital_change_type_id = table.Column<int>(type: "int", nullable: false),
                    position_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_capital_change", x => x.id);
                    table.ForeignKey(
                        name: "FK_capital_change_capital_change_type_capital_change_type_id",
                        column: x => x.capital_change_type_id,
                        principalTable: "capital_change_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_capital_change_position_position_id",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dividend",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    entitlement_date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    no_of_shares_eligible = table.Column<int>(type: "int", nullable: false),
                    dividend_per_share = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    is_subject_to_withholding_tax = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    withholding_tax = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    position_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dividend", x => x.id);
                    table.ForeignKey(
                        name: "FK_dividend_position_position_id",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateOnly>(type: "DATE", nullable: false),
                    no_of_shares_transacted = table.Column<int>(type: "int", nullable: false),
                    transacted_price_per_share = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    total_transaction_related_expenses = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    transaction_type_id = table.Column<int>(type: "int", nullable: false),
                    position_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaction_position_position_id",
                        column: x => x.position_id,
                        principalTable: "position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transaction_transaction_type_transaction_type_id",
                        column: x => x.transaction_type_id,
                        principalTable: "transaction_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "capital_change_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Increase" },
                    { 2, "Decrease" }
                });

            migrationBuilder.InsertData(
                table: "transaction_type",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Buy" },
                    { 2, "Sell" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_capital_change_capital_change_type_id",
                table: "capital_change",
                column: "capital_change_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_capital_change_position_id",
                table: "capital_change",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_dividend_position_id",
                table: "dividend",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_position_stock_id",
                table: "position",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_code",
                table: "stock",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transaction_position_id",
                table: "transaction",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_transaction_type_id",
                table: "transaction",
                column: "transaction_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "capital_change");

            migrationBuilder.DropTable(
                name: "dividend");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "capital_change_type");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropTable(
                name: "transaction_type");

            migrationBuilder.DropTable(
                name: "stock");
        }
    }
}
