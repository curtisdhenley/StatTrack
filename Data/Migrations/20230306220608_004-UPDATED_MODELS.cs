﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StatTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class _004UPDATED_MODELS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TicketAttachments");
        }
    }
}
