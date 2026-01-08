using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenMind.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToGenericOAuthTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new OAuthTokens table
            migrationBuilder.CreateTable(
                name: "OAuthTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scopes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing UserGoogleTokens to OAuthTokens with Provider = 'Google'
            migrationBuilder.Sql(@"
                INSERT INTO ""OAuthTokens"" (""UserId"", ""Provider"", ""AccessToken"", ""RefreshToken"", ""ExpiresAt"", ""CreatedAt"", ""UpdatedAt"", ""Scopes"")
                SELECT ""UserId"", 'Google', ""AccessToken"", ""RefreshToken"", ""ExpiresAt"", ""CreatedAt"", ""UpdatedAt"", ""Scopes""
                FROM ""UserGoogleTokens""
            ");

            // Drop the old table
            migrationBuilder.DropTable(
                name: "UserGoogleTokens");

            // Create unique index on UserId and Provider combination
            migrationBuilder.CreateIndex(
                name: "IX_OAuthTokens_UserId_Provider",
                table: "OAuthTokens",
                columns: new[] { "UserId", "Provider" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Create the old UserGoogleTokens table
            migrationBuilder.CreateTable(
                name: "UserGoogleTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    Scopes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoogleTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGoogleTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate Google tokens back to UserGoogleTokens
            migrationBuilder.Sql(@"
                INSERT INTO ""UserGoogleTokens"" (""UserId"", ""AccessToken"", ""RefreshToken"", ""ExpiresAt"", ""CreatedAt"", ""UpdatedAt"", ""Scopes"")
                SELECT ""UserId"", ""AccessToken"", ""RefreshToken"", ""ExpiresAt"", ""CreatedAt"", ""UpdatedAt"", ""Scopes""
                FROM ""OAuthTokens""
                WHERE ""Provider"" = 'Google'
            ");

            // Create index for old table
            migrationBuilder.CreateIndex(
                name: "IX_UserGoogleTokens_UserId",
                table: "UserGoogleTokens",
                column: "UserId");

            // Drop the new table
            migrationBuilder.DropTable(
                name: "OAuthTokens");
        }
    }
}
