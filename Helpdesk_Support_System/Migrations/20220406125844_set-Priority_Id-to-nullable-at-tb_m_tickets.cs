using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class setPriority_Idtonullableattb_m_tickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_m_tickets_tb_m_priorities_Priority_Id",
                table: "tb_m_tickets");

            migrationBuilder.AlterColumn<int>(
                name: "Priority_Id",
                table: "tb_m_tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_tb_m_tickets_tb_m_priorities_Priority_Id",
                table: "tb_m_tickets",
                column: "Priority_Id",
                principalTable: "tb_m_priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tb_m_tickets_tb_m_priorities_Priority_Id",
                table: "tb_m_tickets");

            migrationBuilder.AlterColumn<int>(
                name: "Priority_Id",
                table: "tb_m_tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tb_m_tickets_tb_m_priorities_Priority_Id",
                table: "tb_m_tickets",
                column: "Priority_Id",
                principalTable: "tb_m_priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
