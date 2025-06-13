using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiRemoteWorkClientBlockChain.Migrations
{
    /// <inheritdoc />
    public partial class TwoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_primary_contact_server",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<string>(type: "text", nullable: false),
                    TimeoutReceive = table.Column<string>(type: "text", nullable: true),
                    TimeoutSend = table.Column<string>(type: "text", nullable: true),
                    DateConnected = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_primary_contact_server", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_primary_contact_server_Id",
                table: "client_primary_contact_server",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_primary_contact_server");
        }
    }
}
