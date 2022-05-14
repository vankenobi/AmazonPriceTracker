﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonPriceTrackerAPI.Persistence.Migrations
{
    public partial class mig_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MailSendingDate",
                table: "TrackedProducts",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailSendingDate",
                table: "TrackedProducts");
        }
    }
}
