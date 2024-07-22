﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseClaimer.Wisedu.EntityFramework.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tenant",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ClaimRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "ClaimRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tenant",
                table: "ClaimRecords",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contact",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Tenant",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ClaimRecords");

            migrationBuilder.DropColumn(
                name: "Contact",
                table: "ClaimRecords");

            migrationBuilder.DropColumn(
                name: "Tenant",
                table: "ClaimRecords");
        }
    }
}