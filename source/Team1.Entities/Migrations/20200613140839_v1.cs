using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Team1.Entities.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Body = table.Column<string>(nullable: false),
                    StartShowingOnDate = table.Column<DateTime>(type: "Date", nullable: false),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnouncementId);
                });

            migrationBuilder.CreateTable(
                name: "APILogs",
                columns: table => new
                {
                    APILogId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestContentBlock = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseContentBlock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetURL = table.Column<string>(maxLength: 200, nullable: false),
                    TransmissionDateTime = table.Column<DateTime>(nullable: false),
                    ResponseDateTime = table.Column<DateTime>(nullable: true),
                    IncomingRequest = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APILogs", x => x.APILogId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Alpha2 = table.Column<string>(maxLength: 2, nullable: false),
                    Alpha3 = table.Column<string>(maxLength: 3, nullable: false),
                    PostalCodeMask = table.Column<string>(maxLength: 10, nullable: true),
                    GoverningDistrictName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    EventDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EventAlternateDate = table.Column<DateTime>(type: "Date", nullable: true),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "LogTypes",
                columns: table => new
                {
                    LogTypeId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTypes", x => x.LogTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MemberTypes",
                columns: table => new
                {
                    MemberTypeId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTypes", x => x.MemberTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MobileCarriers",
                columns: table => new
                {
                    MobileCarrierId = table.Column<int>(nullable: false),
                    MobileCarrierName = table.Column<string>(maxLength: 200, nullable: false),
                    TextingEmailSuffix = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileCarriers", x => x.MobileCarrierId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 200, nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 50, nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TaskCategories",
                columns: table => new
                {
                    TaskCategoryId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCategories", x => x.TaskCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "GoverningDistricts",
                columns: table => new
                {
                    GoverningDistrictId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Code = table.Column<string>(maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoverningDistricts", x => x.GoverningDistrictId);
                    table.ForeignKey(
                        name: "FK_GoverningDistricts_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 200, nullable: false),
                    LastName = table.Column<string>(maxLength: 200, nullable: false),
                    Email = table.Column<string>(maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 25, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    IsLoginEnabled = table.Column<bool>(nullable: false),
                    CertificationLevel = table.Column<byte>(nullable: false),
                    TripoliNumber = table.Column<string>(maxLength: 100, nullable: true),
                    NarNumber = table.Column<string>(maxLength: 100, nullable: true),
                    PaidThroughYear = table.Column<int>(nullable: false),
                    OptOutOfMemberEmails = table.Column<bool>(nullable: false),
                    OptOutOfGeneralEmails = table.Column<bool>(nullable: false),
                    MobileCarrierId = table.Column<int>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 500, nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(maxLength: 200, nullable: true),
                    SecurityStamp = table.Column<string>(maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    LastSignInDateTime = table.Column<DateTimeOffset>(nullable: true),
                    LastPasswordChangeDateTime = table.Column<DateTimeOffset>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: true),
                    AuthyUserId = table.Column<int>(nullable: true),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_MobileCarriers_MobileCarrierId",
                        column: x => x.MobileCarrierId,
                        principalTable: "MobileCarriers",
                        principalColumn: "MobileCarrierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    RoleClaimId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<byte>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 500, nullable: false),
                    ClaimValue = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.RoleClaimId);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskCategoryId = table.Column<byte>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: false),
                    DueMonth = table.Column<byte>(nullable: false),
                    DueDay = table.Column<byte>(nullable: false),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskCategories_TaskCategoryId",
                        column: x => x.TaskCategoryId,
                        principalTable: "TaskCategories",
                        principalColumn: "TaskCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationName = table.Column<string>(maxLength: 200, nullable: false),
                    LocationDescription = table.Column<string>(maxLength: 2000, nullable: false),
                    FAAWaiver = table.Column<string>(maxLength: 200, nullable: true),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    Address1 = table.Column<string>(maxLength: 200, nullable: true),
                    Address2 = table.Column<string>(maxLength: 200, nullable: true),
                    Address3 = table.Column<string>(maxLength: 200, nullable: true),
                    City = table.Column<string>(maxLength: 200, nullable: true),
                    GoverningDistrictId = table.Column<int>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    PostalCode = table.Column<string>(maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                    table.ForeignKey(
                        name: "FK_Locations_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Locations_GoverningDistricts_GoverningDistrictId",
                        column: x => x.GoverningDistrictId,
                        principalTable: "GoverningDistricts",
                        principalColumn: "GoverningDistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    Address1 = table.Column<string>(maxLength: 200, nullable: true),
                    Address2 = table.Column<string>(maxLength: 200, nullable: true),
                    Address3 = table.Column<string>(maxLength: 200, nullable: true),
                    City = table.Column<string>(maxLength: 200, nullable: true),
                    GoverningDistrictId = table.Column<int>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    PostalCode = table.Column<string>(maxLength: 12, nullable: true),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_GoverningDistricts_GoverningDistrictId",
                        column: x => x.GoverningDistrictId,
                        principalTable: "GoverningDistricts",
                        principalColumn: "GoverningDistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    PictureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(nullable: false),
                    ApprovedByUserId = table.Column<int>(nullable: true),
                    ApprovedDateTime = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    InactivatedById = table.Column<int>(nullable: true),
                    InactiveDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<int>(nullable: true),
                    CreatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedById = table.Column<int>(nullable: true),
                    UpdatedDateTime = table.Column<DateTimeOffset>(nullable: true),
                    DocumentFilename = table.Column<string>(maxLength: 1000, nullable: true),
                    DocumentDisplayName = table.Column<string>(maxLength: 500, nullable: true),
                    MimeType = table.Column<string>(maxLength: 200, nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.PictureId);
                    table.ForeignKey(
                        name: "FK_Pictures_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    LogId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    LogGroupKey = table.Column<Guid>(nullable: true),
                    LogTypeId = table.Column<byte>(nullable: false),
                    EventDescription = table.Column<string>(nullable: false),
                    EventDateTime = table.Column<DateTimeOffset>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_SystemLogs_LogTypes_LogTypeId",
                        column: x => x.LogTypeId,
                        principalTable: "LogTypes",
                        principalColumn: "LogTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SystemLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    UserClaimId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 500, nullable: false),
                    ClaimValue = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.UserClaimId);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 2000, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 2000, nullable: false),
                    ProviderDisplayName = table.Column<string>(maxLength: 2000, nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMemberTypes",
                columns: table => new
                {
                    MemberTypeId = table.Column<byte>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMemberTypes", x => new { x.UserId, x.MemberTypeId });
                    table.ForeignKey(
                        name: "FK_UserMemberTypes_MemberTypes_MemberTypeId",
                        column: x => x.MemberTypeId,
                        principalTable: "MemberTypes",
                        principalColumn: "MemberTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserMemberTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RefreshToken = table.Column<string>(maxLength: 100, nullable: true),
                    ExpiresOnDateTime = table.Column<DateTime>(nullable: false),
                    ImpersonationUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_Users_ImpersonationUserId",
                        column: x => x.ImpersonationUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 2000, nullable: false),
                    Name = table.Column<string>(maxLength: 1000, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskMemberTypes",
                columns: table => new
                {
                    MemberTypeId = table.Column<byte>(nullable: false),
                    TaskId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskMemberTypes", x => new { x.TaskId, x.MemberTypeId });
                    table.ForeignKey(
                        name: "FK_TaskMemberTypes_MemberTypes_MemberTypeId",
                        column: x => x.MemberTypeId,
                        principalTable: "MemberTypes",
                        principalColumn: "MemberTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskMemberTypes_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventLocations",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLocations", x => new { x.EventId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_EventLocations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_GoverningDistrictId",
                table: "Addresses",
                column: "GoverningDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLocations_LocationId",
                table: "EventLocations",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GoverningDistricts_CountryId",
                table: "GoverningDistricts",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CountryId",
                table: "Locations",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_GoverningDistrictId",
                table: "Locations",
                column: "GoverningDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Pictures_UserId",
                table: "Pictures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_LogTypeId",
                table: "SystemLogs",
                column: "LogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_UserId",
                table: "SystemLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskMemberTypes_MemberTypeId",
                table: "TaskMemberTypes",
                column: "MemberTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCategoryId",
                table: "Tasks",
                column: "TaskCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMemberTypes_MemberTypeId",
                table: "UserMemberTypes",
                column: "MemberTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_ImpersonationUserId",
                table: "UserRefreshTokens",
                column: "ImpersonationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MobileCarrierId",
                table: "Users",
                column: "MobileCarrierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "APILogs");

            migrationBuilder.DropTable(
                name: "EventLocations");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.DropTable(
                name: "TaskMemberTypes");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserMemberTypes");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "LogTypes");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "MemberTypes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GoverningDistricts");

            migrationBuilder.DropTable(
                name: "TaskCategories");

            migrationBuilder.DropTable(
                name: "MobileCarriers");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
