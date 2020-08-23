using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characteristics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characteristics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradeNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technologies",
                columns: table => new
                {
                    SId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.SId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Information = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Photo = table.Column<byte[]>(nullable: true),
                    Surname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Authorizations_Id",
                        column: x => x.Id,
                        principalTable: "Authorizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataLinks",
                columns: table => new
                {
                    NId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Level = table.Column<int>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    SId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLinks", x => x.NId);
                    table.ForeignKey(
                        name: "FK_DataLinks_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    SId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    NId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => new { x.Id, x.SId });
                    table.ForeignKey(
                        name: "FK_Grades_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnologyCharacteristics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    SId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyCharacteristics", x => new { x.Id, x.SId });
                    table.ForeignKey(
                        name: "FK_TechnologyCharacteristics_Characteristics_Id",
                        column: x => x.Id,
                        principalTable: "Characteristics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnologyCharacteristics_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestLinks",
                columns: table => new
                {
                    NId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Level = table.Column<int>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    SId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLinks", x => x.NId);
                    table.ForeignKey(
                        name: "FK_TestLinks_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Curators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    SId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curators", x => new { x.Id, x.SId });
                    table.ForeignKey(
                        name: "FK_Curators_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Curators_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTechnologies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    SId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTechnologies", x => new { x.Id, x.SId });
                    table.ForeignKey(
                        name: "FK_UserTechnologies_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTechnologies_Technologies_SId",
                        column: x => x.SId,
                        principalTable: "Technologies",
                        principalColumn: "SId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Curators_SId",
                table: "Curators",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_DataLinks_SId",
                table: "DataLinks",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SId",
                table: "Grades",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnologyCharacteristics_SId",
                table: "TechnologyCharacteristics",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLinks_SId",
                table: "TestLinks",
                column: "SId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTechnologies_SId",
                table: "UserTechnologies",
                column: "SId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Curators");

            migrationBuilder.DropTable(
                name: "DataLinks");

            migrationBuilder.DropTable(
                name: "GradeNames");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "TechnologyCharacteristics");

            migrationBuilder.DropTable(
                name: "TestLinks");

            migrationBuilder.DropTable(
                name: "UserTechnologies");

            migrationBuilder.DropTable(
                name: "Characteristics");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Technologies");

            migrationBuilder.DropTable(
                name: "Authorizations");
        }
    }
}
