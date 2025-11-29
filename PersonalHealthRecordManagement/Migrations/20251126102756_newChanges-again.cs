using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalHealthRecordManagement.Migrations
{
    /// <inheritdoc />
    public partial class newChangesagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ASPNETROLES",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZEDNAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    CONCURRENCYSTAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETROLES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETUSERS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    FULLNAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZEDUSERNAME = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    NORMALIZEDEMAIL = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    EMAILCONFIRMED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PASSWORDHASH = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SECURITYSTAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CONCURRENCYSTAMP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PHONENUMBER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PHONENUMBERCONFIRMED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TWOFACTORENABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LOCKOUTEND = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LOCKOUTENABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ACCESSFAILEDCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETUSERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETROLECLAIMS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ROLEID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CLAIMTYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAIMVALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETROLECLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ASPNETROLECLAIMS_ASPNETROLES_ROLEID",
                        column: x => x.ROLEID,
                        principalTable: "ASPNETROLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APPOINTMENTS",
                columns: table => new
                {
                    APPOINTMENTID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    DOCTORNAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PURPOSE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    APPOINTMENTDATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPOINTMENTS", x => x.APPOINTMENTID);
                    table.ForeignKey(
                        name: "FK_APPOINTMENTS_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETUSERCLAIMS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    CLAIMTYPE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CLAIMVALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETUSERCLAIMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ASPNETUSERCLAIMS_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETUSERLOGINS",
                columns: table => new
                {
                    LOGINPROVIDER = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PROVIDERKEY = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PROVIDERDISPLAYNAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETUSERLOGINS", x => new { x.LOGINPROVIDER, x.PROVIDERKEY });
                    table.ForeignKey(
                        name: "FK_ASPNETUSERLOGINS_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETUSERROLES",
                columns: table => new
                {
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    ROLEID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETUSERROLES", x => new { x.USERID, x.ROLEID });
                    table.ForeignKey(
                        name: "FK_ASPNETUSERROLES_ASPNETROLES_ROLEID",
                        column: x => x.ROLEID,
                        principalTable: "ASPNETROLES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ASPNETUSERROLES_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ASPNETUSERTOKENS",
                columns: table => new
                {
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    LOGINPROVIDER = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    VALUE = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASPNETUSERTOKENS", x => new { x.USERID, x.LOGINPROVIDER, x.NAME });
                    table.ForeignKey(
                        name: "FK_ASPNETUSERTOKENS_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MEDICALRECORDS",
                columns: table => new
                {
                    RECORDID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    RECORDTYPE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PROVIDER = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    RECORDDATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    FILEURL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICALRECORDS", x => x.RECORDID);
                    table.ForeignKey(
                        name: "FK_MEDICALRECORDS_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USERPROFILES",
                columns: table => new
                {
                    USERPROFILEID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERID = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    AGE = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    GENDER = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    WEIGHT = table.Column<decimal>(type: "Number(4,2)", nullable: true),
                    BLOODGROUP = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EMERGENCYCONTACT = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERPROFILES", x => x.USERPROFILEID);
                    table.ForeignKey(
                        name: "FK_USERPROFILES_ASPNETUSERS_USERID",
                        column: x => x.USERID,
                        principalTable: "ASPNETUSERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ALLERGIES",
                columns: table => new
                {
                    ALLERGYID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERPROFILEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ALLERGYNAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SYMPTOMS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SEVERITY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLERGIES", x => x.ALLERGYID);
                    table.ForeignKey(
                        name: "FK_ALLERGIES_USERPROFILES_USERPROFILEID",
                        column: x => x.USERPROFILEID,
                        principalTable: "USERPROFILES",
                        principalColumn: "USERPROFILEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MEDICATIONS",
                columns: table => new
                {
                    MEDICATIONID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USERPROFILEID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MEDICINENAME = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FREQUENCY = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    PRESCRIBEDFOR = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PRESCRIBEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DATEPRESCRIBED = table.Column<string>(type: "NVARCHAR2(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICATIONS", x => x.MEDICATIONID);
                    table.ForeignKey(
                        name: "FK_MEDICATIONS_USERPROFILES_USERPROFILEID",
                        column: x => x.USERPROFILEID,
                        principalTable: "USERPROFILES",
                        principalColumn: "USERPROFILEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALLERGIES_USERPROFILEID",
                table: "ALLERGIES",
                column: "USERPROFILEID");

            migrationBuilder.CreateIndex(
                name: "IX_APPOINTMENTS_USERID",
                table: "APPOINTMENTS",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_ASPNETROLECLAIMS_ROLEID",
                table: "ASPNETROLECLAIMS",
                column: "ROLEID");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "ASPNETROLES",
                column: "NORMALIZEDNAME",
                unique: true,
                filter: "\"NORMALIZEDNAME\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ASPNETUSERCLAIMS_USERID",
                table: "ASPNETUSERCLAIMS",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_ASPNETUSERLOGINS_USERID",
                table: "ASPNETUSERLOGINS",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_ASPNETUSERROLES_ROLEID",
                table: "ASPNETUSERROLES",
                column: "ROLEID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ASPNETUSERS",
                column: "NORMALIZEDEMAIL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ASPNETUSERS",
                column: "NORMALIZEDUSERNAME",
                unique: true,
                filter: "\"NORMALIZEDUSERNAME\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MEDICALRECORDS_USERID",
                table: "MEDICALRECORDS",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_MEDICATIONS_USERPROFILEID",
                table: "MEDICATIONS",
                column: "USERPROFILEID");

            migrationBuilder.CreateIndex(
                name: "IX_USERPROFILES_USERID",
                table: "USERPROFILES",
                column: "USERID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALLERGIES");

            migrationBuilder.DropTable(
                name: "APPOINTMENTS");

            migrationBuilder.DropTable(
                name: "ASPNETROLECLAIMS");

            migrationBuilder.DropTable(
                name: "ASPNETUSERCLAIMS");

            migrationBuilder.DropTable(
                name: "ASPNETUSERLOGINS");

            migrationBuilder.DropTable(
                name: "ASPNETUSERROLES");

            migrationBuilder.DropTable(
                name: "ASPNETUSERTOKENS");

            migrationBuilder.DropTable(
                name: "MEDICALRECORDS");

            migrationBuilder.DropTable(
                name: "MEDICATIONS");

            migrationBuilder.DropTable(
                name: "ASPNETROLES");

            migrationBuilder.DropTable(
                name: "USERPROFILES");

            migrationBuilder.DropTable(
                name: "ASPNETUSERS");
        }
    }
}
