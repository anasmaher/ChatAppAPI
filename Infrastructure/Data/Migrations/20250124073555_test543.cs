using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test543 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Messages_Messageid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Messageid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "ConversationMembers");

            migrationBuilder.DropColumn(
                name: "Messageid",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Messages",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Messages",
                newName: "id");

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Conversations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "ConversationMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Messageid",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Messageid",
                table: "AspNetUsers",
                column: "Messageid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Messages_Messageid",
                table: "AspNetUsers",
                column: "Messageid",
                principalTable: "Messages",
                principalColumn: "id");
        }
    }
}
