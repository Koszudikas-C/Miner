using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiRemoteWorkClientBlockChain.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client_primary_contact_server",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    IpLocal = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Port = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    AttemptsConnection = table.Column<int>(type: "integer", nullable: false),
                    StateClient = table.Column<int>(type: "integer", nullable: false),
                    TimeoutReceive = table.Column<int>(type: "integer", maxLength: 8, nullable: true),
                    TimeoutSend = table.Column<int>(type: "integer", maxLength: 8, nullable: true),
                    PrimaryConnection = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateConnected = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_primary_contact_server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "nonce_used_auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuidTokenGlobal = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonce_used_auth", x => x.Id);
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

            migrationBuilder.DropTable(
                name: "nonce_used_auth");
        }
    }
}
