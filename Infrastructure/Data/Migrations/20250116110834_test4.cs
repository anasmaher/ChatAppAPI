using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_ActionUserId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_ActionUserId",
                table: "Friendship",
                column: "ActionUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_ActionUserId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_ActionUserId",
                table: "Friendship",
                column: "ActionUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User1Id",
                table: "Friendship",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_User2Id",
                table: "Friendship",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
