using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopingCarts_AspNetUsers_ApplicationUserId",
                table: "ShopingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShopingCarts_ApplicationUserId",
                table: "ShopingCarts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ShopingCarts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShopingCarts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ShopingCarts_UserId",
                table: "ShopingCarts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopingCarts_AspNetUsers_UserId",
                table: "ShopingCarts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopingCarts_AspNetUsers_UserId",
                table: "ShopingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShopingCarts_UserId",
                table: "ShopingCarts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShopingCarts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShopingCarts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopingCarts_ApplicationUserId",
                table: "ShopingCarts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopingCarts_AspNetUsers_ApplicationUserId",
                table: "ShopingCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
