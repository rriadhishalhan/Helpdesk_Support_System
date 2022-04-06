using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class createmultipletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_m_categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_m_customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    First_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone_number = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_m_positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_m_priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_m_employees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    First_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Workload = table.Column<int>(type: "int", nullable: false),
                    Phone_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_m_employees_tb_m_positions_Position_id",
                        column: x => x.Position_id,
                        principalTable: "tb_m_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_m_tickets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Issue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Category_Id = table.Column<int>(type: "int", nullable: false),
                    Priority_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_m_tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_m_tickets_tb_m_categories_Category_Id",
                        column: x => x.Category_Id,
                        principalTable: "tb_m_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tb_m_tickets_tb_m_customers_Customer_Id",
                        column: x => x.Customer_Id,
                        principalTable: "tb_m_customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_m_tickets_tb_m_priorities_Priority_Id",
                        column: x => x.Priority_Id,
                        principalTable: "tb_m_priorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tb_tr_ticketHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ticket_Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Employee_Id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_tr_ticketHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_tr_ticketHistory_tb_m_employees_Employee_Id",
                        column: x => x.Employee_Id,
                        principalTable: "tb_m_employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_tr_ticketHistory_tb_m_tickets_Ticket_Id",
                        column: x => x.Ticket_Id,
                        principalTable: "tb_m_tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_employees_Position_id",
                table: "tb_m_employees",
                column: "Position_id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_tickets_Category_Id",
                table: "tb_m_tickets",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_tickets_Customer_Id",
                table: "tb_m_tickets",
                column: "Customer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_m_tickets_Priority_Id",
                table: "tb_m_tickets",
                column: "Priority_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_tr_ticketHistory_Employee_Id",
                table: "tb_tr_ticketHistory",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_tb_tr_ticketHistory_Ticket_Id",
                table: "tb_tr_ticketHistory",
                column: "Ticket_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_tr_ticketHistory");

            migrationBuilder.DropTable(
                name: "tb_m_employees");

            migrationBuilder.DropTable(
                name: "tb_m_tickets");

            migrationBuilder.DropTable(
                name: "tb_m_positions");

            migrationBuilder.DropTable(
                name: "tb_m_categories");

            migrationBuilder.DropTable(
                name: "tb_m_customers");

            migrationBuilder.DropTable(
                name: "tb_m_priorities");
        }
    }
}
