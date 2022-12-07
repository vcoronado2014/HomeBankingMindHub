using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBankingMindHub.Migrations
{
    public partial class addLoanEntityPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payments",
                table: "ClientLoans",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payments",
                table: "ClientLoans");
        }
    }
}
