using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_CDTT_NguyenMinhDat.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCouponData204 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Coupons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Coupons");
        }
    }
}
