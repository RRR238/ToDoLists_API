using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo_lists.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "usersListsLinks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "toDoLists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToDoListName",
                table: "toDoLists",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "username",
                table: "usersListsLinks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "toDoLists");

            migrationBuilder.DropColumn(
                name: "ToDoListName",
                table: "toDoLists");
        }
    }
}
