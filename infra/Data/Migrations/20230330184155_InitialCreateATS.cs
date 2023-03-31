using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateATS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssessmentQBank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQBank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentStandardQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentParameter = table.Column<string>(type: "TEXT", nullable: false),
                    QNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentStandardQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    FirstName = table.Column<string>(type: "VARCHAR", unicode: false, maxLength: 75, nullable: false),
                    SecondName = table.Column<string>(type: "VARCHAR", unicode: false, maxLength: 75, nullable: true),
                    FamilyName = table.Column<string>(type: "VARCHAR", unicode: false, maxLength: 75, nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: false),
                    ReferredBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    DOB = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    AadharNo = table.Column<string>(type: "TEXT", nullable: true),
                    PpNo = table.Column<string>(type: "TEXT", nullable: true),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    District = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActive = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Introduction = table.Column<string>(type: "TEXT", nullable: true),
                    Interests = table.Column<string>(type: "TEXT", nullable: true),
                    AppUserIdNotEnforced = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationDesired = table.Column<bool>(type: "INTEGER", nullable: false),
                    CandidateStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Divn = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    AccountType = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountClass = table.Column<string>(type: "TEXT", nullable: true),
                    OpBalance = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COAs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResultId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RvwStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleasedForProduction = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviewDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerReviewStatusName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviewDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    KnownAs = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Add = table.Column<string>(type: "TEXT", nullable: true),
                    Add2 = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    District = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Phone2 = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Introduction = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CVRefRestrictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    restrictionReason = table.Column<int>(type: "INTEGER", nullable: false),
                    RestrictedById = table.Column<int>(type: "INTEGER", nullable: false),
                    RestrictedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RestrictionLifted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RestrictionLiftedById = table.Column<int>(type: "INTEGER", nullable: false),
                    RestrictionLiftedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVRefRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CVReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HRExecutiveId = table.Column<int>(type: "INTEGER", nullable: false),
                    HRExecTaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChecklistHRId = table.Column<int>(type: "INTEGER", nullable: true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubmittedByHRExecOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HRExecRemarks = table.Column<string>(type: "TEXT", nullable: true),
                    NoReviewBySupervisor = table.Column<bool>(type: "INTEGER", nullable: false),
                    HRSupId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedBySupOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SupReviewResultId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    SupRemarks = table.Column<string>(type: "TEXT", nullable: true),
                    HRMId = table.Column<int>(type: "INTEGER", nullable: true),
                    HRMReviewedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HRMReviewResultId = table.Column<int>(type: "INTEGER", nullable: true),
                    HRMTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    HRMRemarks = table.Column<string>(type: "TEXT", nullable: true),
                    DocControllerAdminEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocControllerAdminTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    CVReferredOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeployStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedDaysToCompleteThisStage = table.Column<int>(type: "INTEGER", nullable: false),
                    NextDeployStageSequence = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeployStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StageId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ProcessName = table.Column<string>(type: "TEXT", nullable: false),
                    NextStageId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkingDaysReqdForNextStage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageGroup = table.Column<string>(type: "TEXT", nullable: true),
                    SenderEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    SenderUserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecipientUserName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecipientEmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    CcEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    BccEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    MessageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    MessageComposedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateReadOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MessageSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SenderDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecipientDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    SenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    PostAction = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    SecondName = table.Column<string>(type: "TEXT", nullable: true),
                    FamilyName = table.Column<string>(type: "TEXT", nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    AadharNo = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastWorkingDay = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActive = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilesOnFileSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    FileType = table.Column<string>(type: "TEXT", nullable: true),
                    Extension = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesOnFileSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileUploads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttachmentType = table.Column<string>(type: "TEXT", nullable: true),
                    Length = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedLocation = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedbyUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Divn = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    CoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    VoucherNo = table.Column<int>(type: "INTEGER", maxLength: 10, nullable: false),
                    VoucherDated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedById = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedByName = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Helps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Topic = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAttendancesStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAttendancesStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewMode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    InterviewerName = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewVenue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InterviewDateFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InterviewLeaderId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerRepresentative = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludingRemarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageComposeSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageType = table.Column<string>(type: "TEXT", nullable: true),
                    Mode = table.Column<string>(type: "TEXT", nullable: true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    LineText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageComposeSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNo = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    DateOfAdvise = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TextAdvised = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    CompanyAdvised = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialAdvised = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProspectiveCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CategoryRef = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Natioality = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeTitle = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CandidateName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Age = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    AlternatePhoneNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    AlternateEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentLocation = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Education = table.Column<string>(type: "TEXT", nullable: true),
                    Ctc = table.Column<string>(type: "TEXT", nullable: true),
                    WorkExperience = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatusByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectiveCandidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewItemDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    IsResponseBoolean = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItemDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewItemStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItemStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelectionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    DecisionType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SMSMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SequenceNo = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComposedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SMSDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    SMSText = table.Column<string>(type: "TEXT", nullable: true),
                    DeliveryResult = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CVReviewId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskDate = table.Column<DateTime>(type: "TEXT", maxLength: 250, nullable: false),
                    TaskOwnerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskOwnerName = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedToId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedToName = table.Column<string>(type: "TEXT", nullable: true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonType = table.Column<string>(type: "TEXT", nullable: true),
                    TaskDescription = table.Column<string>(type: "TEXT", nullable: false),
                    CompleteBy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TaskStatus = table.Column<string>(type: "TEXT", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HistoryItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    PostTaskAction = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserHistoryHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryRefCode = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryRefName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CompleteBy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssignedToId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedToName = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedById = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedByName = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Concluded = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistoryHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQBankItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentQBankId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "TEXT", nullable: false),
                    QNo = table.Column<int>(type: "INTEGER", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQBankItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentQBankItems_AssessmentQBank_AssessmentQBankId",
                        column: x => x.AssessmentQBankId,
                        principalTable: "AssessmentQBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressType = table.Column<string>(type: "TEXT", nullable: true),
                    Add = table.Column<string>(type: "TEXT", unicode: false, maxLength: 250, nullable: false),
                    StreetAdd = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    District = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityAddresses_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    AttachmentType = table.Column<string>(type: "TEXT", nullable: true),
                    AttachmentSizeInBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: true),
                    url = table.Column<string>(type: "TEXT", nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedByEmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAttachments_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserExps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Employer = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentJob = table.Column<bool>(type: "INTEGER", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    MonthlySalaryDrawn = table.Column<int>(type: "INTEGER", nullable: true),
                    WorkedFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkedUpto = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExps_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPassports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    PassportNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Validity = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPassports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPassports_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    MobileNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhones_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Profession = table.Column<string>(type: "TEXT", nullable: true),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfessions_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    QualificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQualifications_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewTransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerReviewDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedBySup = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApprovedById = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReviewItems_CustomerReviews_CustomerReviewId",
                        column: x => x.CustomerReviewId,
                        principalTable: "CustomerReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgencySpecialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencySpecialties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgencySpecialties_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerIndustries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerIndustries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerIndustries_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOfficials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LogInCredential = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: true),
                    Designation = table.Column<string>(type: "TEXT", nullable: true),
                    Divn = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomerId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOfficials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOfficials_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerOfficials_Customers_CustomerId1",
                        column: x => x.CustomerId1,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    BuyerEmail = table.Column<string>(type: "TEXT", nullable: true),
                    OrderRef = table.Column<string>(type: "TEXT", nullable: true),
                    OrderRefDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SalesmanName = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectManagerId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicalProcessInchargeEmpId = table.Column<int>(type: "INTEGER", nullable: true),
                    VisaProcessInchargeEmpId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmigProcessInchargeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TravelProcessInchargeId = table.Column<int>(type: "INTEGER", nullable: true),
                    SalesmanId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompleteBy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    CityOfWorking = table.Column<string>(type: "TEXT", nullable: false),
                    ContractReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractReviewStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedRevenue = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    ForwardedToHRDeptOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CVRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    HRExecId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    ReferredOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeployStageId = table.Column<int>(type: "INTEGER", nullable: true),
                    NextStageId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeployStageDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentIntentId = table.Column<string>(type: "TEXT", nullable: true),
                    RefStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    RefStatusDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CVRefs_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CVRefs_DeployStages_DeployStageId",
                        column: x => x.DeployStageId,
                        principalTable: "DeployStages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressType = table.Column<string>(type: "TEXT", nullable: true),
                    Add = table.Column<string>(type: "TEXT", nullable: true),
                    StreetAdd = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    District = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAddresses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeHRSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeHRSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeHRSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeOtherSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeOtherSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeOtherSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    MobileNo = table.Column<string>(type: "TEXT", nullable: true),
                    IsOfficial = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeePhones_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    QualificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeQualifications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoucherAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinanceVoucherId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttachmentSizeInBytes = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedByEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    FinanceVoucherId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_FinanceVouchers_FinanceVoucherId",
                        column: x => x.FinanceVoucherId,
                        principalTable: "FinanceVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_FinanceVouchers_FinanceVoucherId1",
                        column: x => x.FinanceVoucherId1,
                        principalTable: "FinanceVouchers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoucherEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinanceVoucherId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CoaId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    Dr = table.Column<long>(type: "INTEGER", nullable: false),
                    Cr = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherEntries_FinanceVouchers_FinanceVoucherId",
                        column: x => x.FinanceVoucherId,
                        principalTable: "FinanceVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    GroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_Connections_Groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "Groups",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "HelpItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HelpId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    HelpText = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpItems_Helps_HelpId",
                        column: x => x.HelpId,
                        principalTable: "Helps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Venue = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewDateFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InterviewMode = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewerName = table.Column<string>(type: "TEXT", nullable: true),
                    InterviewStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludingRemarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItems_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemAssessmentQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderAssessmentItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxMarks = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderItemAssessmentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAssessmentQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemAssessmentQs_OrderItemAssessments_OrderItemAssessmentId",
                        column: x => x.OrderItemAssessmentId,
                        principalTable: "OrderItemAssessments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationTaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TaskTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskTypeName = table.Column<string>(type: "TEXT", nullable: true),
                    TaskStatus = table.Column<string>(type: "TEXT", nullable: false),
                    TaskItemDescription = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: true),
                    NextFollowupOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextFollowupById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_Tasks_ApplicationTaskId",
                        column: x => x.ApplicationTaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Age = table.Column<string>(type: "TEXT", nullable: true),
                    UserHistoryHeaderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Checked = table.Column<bool>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryRef = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentLocation = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PersonType = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    EmailId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AlternateEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    MobileNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    AlternatePhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    Education = table.Column<string>(type: "TEXT", nullable: true),
                    CTC = table.Column<string>(type: "TEXT", nullable: true),
                    WorkExperience = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Concluded = table.Column<bool>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatusByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ConcludedById = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHistories_UserHistoryHeaders_UserHistoryHeaderId",
                        column: x => x.UserHistoryHeaderId,
                        principalTable: "UserHistoryHeaders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DLForwardToAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    customerName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerCity = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectManagerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DLForwardToAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DLForwardToAgents_CustomerOfficials_CustomerOfficialId",
                        column: x => x.CustomerOfficialId,
                        principalTable: "CustomerOfficials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DLForwardToAgents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deploys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StageId = table.Column<int>(type: "INTEGER", nullable: false),
                    NextStageId = table.Column<int>(type: "INTEGER", nullable: false),
                    NextEstimatedStageDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CVRefId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deploys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deploys_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deploys_CVRefs_CVRefId1",
                        column: x => x.CVRefId1,
                        principalTable: "CVRefs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SelectionDecisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SelectionStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectionDecisions_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HelpSubItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HelpId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    HelpText = table.Column<string>(type: "TEXT", nullable: true),
                    HelpItemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpSubItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpSubItems_HelpItems_HelpItemId",
                        column: x => x.HelpItemId,
                        principalTable: "HelpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItemCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterviewItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    PassportNo = table.Column<string>(type: "TEXT", nullable: true),
                    ScheduledFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledUpto = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InterviewMode = table.Column<string>(type: "TEXT", nullable: true),
                    ReportedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InterviewedDateTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AttendanceStatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    SelectionStatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    ConcludingRemarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItemCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItemCandidates_InterviewItems_InterviewItemId",
                        column: x => x.InterviewItemId,
                        principalTable: "InterviewItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHistoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserHistoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryRef = table.Column<string>(type: "TEXT", nullable: true),
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonType = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfContact = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LoggedInUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoggedInUserName = table.Column<string>(type: "TEXT", nullable: true),
                    ContactResultId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactResultName = table.Column<string>(type: "TEXT", nullable: true),
                    GistOfDiscussions = table.Column<string>(type: "TEXT", nullable: true),
                    ComposeSMS = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                        column: x => x.UserHistoryId,
                        principalTable: "UserHistories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DLForwardCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DLForwardToAgentId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DLForwardCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DLForwardCategories_DLForwardToAgents_DLForwardToAgentId",
                        column: x => x.DLForwardToAgentId,
                        principalTable: "DLForwardToAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectionDecisionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    Salary = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    HousingAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    OtherAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    OfferAcceptedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OfferAttachmentUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OfferAcceptanceUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                        column: x => x.SelectionDecisionId,
                        principalTable: "SelectionDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItemCandidatesFollowup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InterviewItemCandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContactedById = table.Column<int>(type: "INTEGER", nullable: false),
                    MobileNoCalled = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    AttendanceStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    FollowupConcluded = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItemCandidatesFollowup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItemCandidatesFollowup_InterviewItemCandidates_InterviewItemCandidateId",
                        column: x => x.InterviewItemCandidateId,
                        principalTable: "InterviewItemCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DLForwardCategoryOfficials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DLForwardCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: false),
                    AgentName = table.Column<string>(type: "TEXT", nullable: true),
                    DateTimeForwarded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateOnlyForwarded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EmailIdForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNoForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    WhatsAppNoForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    LoggedInEmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DLForwardCategoryOfficials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DLForwardCategoryOfficials_DLForwardCategories_DLForwardCategoryId",
                        column: x => x.DLForwardCategoryId,
                        principalTable: "DLForwardCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateAssessmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    Assessed = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateAssessmentItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedById = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedByName = table.Column<string>(type: "TEXT", nullable: true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    requireInternalReview = table.Column<bool>(type: "INTEGER", nullable: false),
                    HrChecklistId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssessResult = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    CvRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskIdDocControllerAdmin = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChecklistHRId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Accepts = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: true),
                    Exceptions = table.Column<string>(type: "TEXT", nullable: true),
                    MandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HrExecComments = table.Column<string>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    ChargesAgreed = table.Column<int>(type: "INTEGER", nullable: false),
                    ExceptionApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExceptionApprovedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionApprovedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChecklistedOk = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistHRs_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireAssess = table.Column<bool>(type: "INTEGER", nullable: false),
                    SourceFrom = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewItemStatus = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractReviewItems_ContractReviews_ContractReviewId",
                        column: x => x.ContractReviewId,
                        principalTable: "ContractReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractReviewItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Response = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResponseText = table.Column<string>(type: "TEXT", nullable: true),
                    IsResponseBoolean = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewItems_ContractReviewItems_ContractReviewItemId",
                        column: x => x.ContractReviewItemId,
                        principalTable: "ContractReviewItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    JobDescInBrief = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    QualificationDesired = table.Column<string>(type: "TEXT", nullable: true),
                    ExpDesiredMin = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpDesiredMax = table.Column<int>(type: "INTEGER", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryName = table.Column<string>(type: "TEXT", nullable: true),
                    SourceFrom = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCVs = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCVs = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsProcessingOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireInternalReview = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireAssess = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompleteBefore = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HrExecId = table.Column<int>(type: "INTEGER", nullable: true),
                    HRExecName = table.Column<string>(type: "TEXT", nullable: true),
                    NoReviewBySupervisor = table.Column<bool>(type: "INTEGER", nullable: false),
                    HrSupId = table.Column<int>(type: "INTEGER", nullable: true),
                    HrSupName = table.Column<string>(type: "TEXT", nullable: true),
                    HrmId = table.Column<int>(type: "INTEGER", nullable: true),
                    HrmName = table.Column<string>(type: "TEXT", nullable: true),
                    AssignedId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedToName = table.Column<string>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    FeeFromClientINR = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Checked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReviewItemStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    JobDescriptionId = table.Column<int>(type: "INTEGER", nullable: true),
                    RemunerationId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_CVRefs_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CVRefs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Employees_AssignedId",
                        column: x => x.AssignedId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_JobDescriptions_JobDescriptionId",
                        column: x => x.JobDescriptionId,
                        principalTable: "JobDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Remunerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkHours = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    SalaryMin = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryMax = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    HousingAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    OtherAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remunerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remunerations_OrderItems_OrderItemId1",
                        column: x => x.OrderItemId1,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencySpecialties_CustomerId",
                table: "AgencySpecialties",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQBank_CategoryId",
                table: "AssessmentQBank",
                column: "CategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQBankItems_AssessmentQBankId_QNo",
                table: "AssessmentQBankItems",
                columns: new[] { "AssessmentQBankId", "QNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStandardQs_QNo",
                table: "AssessmentStandardQs",
                column: "QNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStandardQs_Question",
                table: "AssessmentStandardQs",
                column: "Question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessmentItems_CandidateAssessmentId",
                table: "CandidateAssessmentItems",
                column: "CandidateAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_CandidateId_OrderItemId",
                table: "CandidateAssessments",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_OrderItemId",
                table: "CandidateAssessments",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_AadharNo",
                table: "Candidates",
                column: "AadharNo",
                unique: true,
                filter: "[AadharNo] != '' AND [AadharNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_ApplicationNo",
                table: "Candidates",
                column: "ApplicationNo",
                unique: true,
                filter: "[ApplicationNo] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_PpNo",
                table: "Candidates",
                column: "PpNo",
                unique: true,
                filter: "[PpNo] != '' AND [PpNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRDatas_Parameter",
                table: "ChecklistHRDatas",
                column: "Parameter",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRDatas_SrNo",
                table: "ChecklistHRDatas",
                column: "SrNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRItems_ChecklistHRId",
                table: "ChecklistHRItems",
                column: "ChecklistHRId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_CandidateId_OrderItemId",
                table: "ChecklistHRs",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_COAs_AccountName",
                table: "COAs",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connections_GroupName",
                table: "Connections",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ContactResults_Name_ResultId",
                table: "ContactResults",
                columns: new[] { "Name", "ResultId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_ContractReviewId",
                table: "ContractReviewItems",
                column: "ContractReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviews_OrderId",
                table: "ContractReviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviews_OrderNo",
                table: "ContractReviews",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIndustries_CustomerId_IndustryId",
                table: "CustomerIndustries",
                columns: new[] { "CustomerId", "IndustryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId",
                table: "CustomerOfficials",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId1",
                table: "CustomerOfficials",
                column: "CustomerId1");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviewItems_CustomerReviewId",
                table: "CustomerReviewItems",
                column: "CustomerReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers",
                columns: new[] { "CustomerName", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVRefs_CandidateId_OrderItemId",
                table: "CVRefs",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVRefs_DeployStageId",
                table: "CVRefs",
                column: "DeployStageId");

            migrationBuilder.CreateIndex(
                name: "IX_CVRefs_OrderItemId",
                table: "CVRefs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CVReviews_CandidateId_OrderItemId",
                table: "CVReviews",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deploys_CVRefId",
                table: "Deploys",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_Deploys_CVRefId1",
                table: "Deploys",
                column: "CVRefId1");

            migrationBuilder.CreateIndex(
                name: "IX_DeployStages_Sequence",
                table: "DeployStages",
                column: "Sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeployStages_Status",
                table: "DeployStages",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeployStatus_StatusName",
                table: "DeployStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardCategories_DLForwardToAgentId",
                table: "DLForwardCategories",
                column: "DLForwardToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardCategories_OrderItemId",
                table: "DLForwardCategories",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardCategoryOfficials_DLForwardCategoryId_DateOnlyForwarded_CustomerOfficialId",
                table: "DLForwardCategoryOfficials",
                columns: new[] { "DLForwardCategoryId", "DateOnlyForwarded", "CustomerOfficialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardToAgents_CustomerOfficialId",
                table: "DLForwardToAgents",
                column: "CustomerOfficialId");

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardToAgents_OrderId",
                table: "DLForwardToAgents",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAddresses_EmployeeId",
                table: "EmployeeAddresses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeHRSkills_EmployeeId_CategoryId_IndustryId",
                table: "EmployeeHRSkills",
                columns: new[] { "EmployeeId", "CategoryId", "IndustryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOtherSkills_EmployeeId_SkillDataId",
                table: "EmployeeOtherSkills",
                columns: new[] { "EmployeeId", "SkillDataId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePhones_EmployeeId",
                table: "EmployeePhones",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeQualifications_EmployeeId_QualificationId",
                table: "EmployeeQualifications",
                columns: new[] { "EmployeeId", "QualificationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_CVRefId",
                table: "Employments",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityAddresses_CandidateId",
                table: "EntityAddresses",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceVouchers_VoucherDated",
                table: "FinanceVouchers",
                column: "VoucherDated");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceVouchers_VoucherNo",
                table: "FinanceVouchers",
                column: "VoucherNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpItems_HelpId",
                table: "HelpItems",
                column: "HelpId");

            migrationBuilder.CreateIndex(
                name: "IX_HelpItems_Sequence_HelpId",
                table: "HelpItems",
                columns: new[] { "Sequence", "HelpId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helps_Topic",
                table: "Helps",
                column: "Topic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpSubItems_HelpItemId",
                table: "HelpSubItems",
                column: "HelpItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_Name",
                table: "Industries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttendancesStatus_Status",
                table: "InterviewAttendancesStatus",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidates_ApplicationNo",
                table: "InterviewItemCandidates",
                column: "ApplicationNo");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidates_CandidateId_InterviewItemId",
                table: "InterviewItemCandidates",
                columns: new[] { "CandidateId", "InterviewItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidates_InterviewItemId",
                table: "InterviewItemCandidates",
                column: "InterviewItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidatesFollowup_InterviewItemCandidateId",
                table: "InterviewItemCandidatesFollowup",
                column: "InterviewItemCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_InterviewId",
                table: "InterviewItems",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_OrderItemId",
                table: "InterviewItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_OrderId",
                table: "Interviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_OrderNo",
                table: "Interviews",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_OrderItemId",
                table: "JobDescriptions",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_OrderItemId1",
                table: "JobDescriptions",
                column: "OrderItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_AssignedId",
                table: "OrderItems",
                column: "AssignedId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CategoryId",
                table: "OrderItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_JobDescriptionId",
                table: "OrderItems",
                column: "JobDescriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderItemId",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_RemunerationId",
                table: "OrderItems",
                column: "RemunerationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CandidateId",
                table: "Photos",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_ResumeId",
                table: "ProspectiveCandidates",
                column: "ResumeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_Name",
                table: "Qualifications",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remunerations_OrderItemId",
                table: "Remunerations",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remunerations_OrderItemId1",
                table: "Remunerations",
                column: "OrderItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItemDatas_SrNo",
                table: "ReviewItemDatas",
                column: "SrNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItems_ContractReviewItemId_ReviewParameter",
                table: "ReviewItems",
                columns: new[] { "ContractReviewItemId", "ReviewParameter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItems_ContractReviewItemId_SrNo",
                table: "ReviewItems",
                columns: new[] { "ContractReviewItemId", "SrNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItemStatuses_ItemStatus",
                table: "ReviewItemStatuses",
                column: "ItemStatus",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStatuses_Status",
                table: "ReviewStatuses",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillDatas_SkillName",
                table: "SkillDatas",
                column: "SkillName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ApplicationTaskId",
                table: "TaskItems",
                column: "ApplicationTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TransactionDate",
                table: "TaskItems",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_UserId",
                table: "TaskItems",
                column: "UserId",
                filter: "[UserId] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId_OrderItemId_CandidateId_TaskTypeId",
                table: "Tasks",
                columns: new[] { "AssignedToId", "OrderItemId", "CandidateId", "TaskTypeId" },
                unique: true,
                filter: "CandidateId > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ResumeId",
                table: "Tasks",
                column: "ResumeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskOwnerId",
                table: "Tasks",
                column: "TaskOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskTypeId",
                table: "Tasks",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskTypeId_OrderId",
                table: "Tasks",
                columns: new[] { "TaskTypeId", "OrderId" },
                unique: true,
                filter: "TaskTypeId=14");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttachments_CandidateId",
                table: "UserAttachments",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExps_CandidateId",
                table: "UserExps",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_EmailId",
                table: "UserHistories",
                column: "EmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_MobileNo",
                table: "UserHistories",
                column: "MobileNo",
                unique: true,
                filter: "MobileNo != ''");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_PersonType",
                table: "UserHistories",
                column: "PersonType");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_UserHistoryHeaderId",
                table: "UserHistories",
                column: "UserHistoryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistoryItems_UserHistoryId",
                table: "UserHistoryItems",
                column: "UserHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPassports_CandidateId",
                table: "UserPassports",
                column: "CandidateId",
                filter: "[IsValid]=1");

            migrationBuilder.CreateIndex(
                name: "IX_UserPassports_PassportNo",
                table: "UserPassports",
                column: "PassportNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPhones_CandidateId",
                table: "UserPhones",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfessions_CandidateId",
                table: "UserProfessions",
                column: "CandidateId",
                filter: "[IsMain]=1");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfessions_CandidateId_CategoryId_IndustryId",
                table: "UserProfessions",
                columns: new[] { "CandidateId", "CategoryId", "IndustryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserQualifications_CandidateId",
                table: "UserQualifications",
                column: "CandidateId",
                filter: "[IsMain]=1");

            migrationBuilder.CreateIndex(
                name: "IX_UserQualifications_CandidateId_QualificationId",
                table: "UserQualifications",
                columns: new[] { "CandidateId", "QualificationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FinanceVoucherId",
                table: "VoucherAttachments",
                column: "FinanceVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FinanceVoucherId1",
                table: "VoucherAttachments",
                column: "FinanceVoucherId1");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherEntries_CoaId",
                table: "VoucherEntries",
                column: "CoaId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherEntries_FinanceVoucherId",
                table: "VoucherEntries",
                column: "FinanceVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherEntries_TransDate",
                table: "VoucherEntries",
                column: "TransDate");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAssessmentItems_CandidateAssessments_CandidateAssessmentId",
                table: "CandidateAssessmentItems",
                column: "CandidateAssessmentId",
                principalTable: "CandidateAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateAssessments_OrderItems_OrderItemId",
                table: "CandidateAssessments",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRItems_ChecklistHRs_ChecklistHRId",
                table: "ChecklistHRItems",
                column: "ChecklistHRId",
                principalTable: "ChecklistHRs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractReviewItems_OrderItems_OrderItemId",
                table: "ContractReviewItems",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobDescriptions_OrderItems_OrderItemId1",
                table: "JobDescriptions",
                column: "OrderItemId1",
                principalTable: "OrderItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Remunerations_RemunerationId",
                table: "OrderItems",
                column: "RemunerationId",
                principalTable: "Remunerations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_JobDescriptions_OrderItems_OrderItemId1",
                table: "JobDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Remunerations_OrderItems_OrderItemId1",
                table: "Remunerations");

            migrationBuilder.DropTable(
                name: "AgencySpecialties");

            migrationBuilder.DropTable(
                name: "AssessmentQBankItems");

            migrationBuilder.DropTable(
                name: "AssessmentStandardQs");

            migrationBuilder.DropTable(
                name: "CandidateAssessmentItems");

            migrationBuilder.DropTable(
                name: "ChecklistHRDatas");

            migrationBuilder.DropTable(
                name: "ChecklistHRItems");

            migrationBuilder.DropTable(
                name: "COAs");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "ContactResults");

            migrationBuilder.DropTable(
                name: "CustomerIndustries");

            migrationBuilder.DropTable(
                name: "CustomerReviewDatas");

            migrationBuilder.DropTable(
                name: "CustomerReviewItems");

            migrationBuilder.DropTable(
                name: "CVRefRestrictions");

            migrationBuilder.DropTable(
                name: "CVReviews");

            migrationBuilder.DropTable(
                name: "Deploys");

            migrationBuilder.DropTable(
                name: "DeployStatus");

            migrationBuilder.DropTable(
                name: "DLForwardCategoryOfficials");

            migrationBuilder.DropTable(
                name: "EmailMessages");

            migrationBuilder.DropTable(
                name: "EmployeeAddresses");

            migrationBuilder.DropTable(
                name: "EmployeeHRSkills");

            migrationBuilder.DropTable(
                name: "EmployeeOtherSkills");

            migrationBuilder.DropTable(
                name: "EmployeePhones");

            migrationBuilder.DropTable(
                name: "EmployeeQualifications");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "EntityAddresses");

            migrationBuilder.DropTable(
                name: "FilesOnFileSystem");

            migrationBuilder.DropTable(
                name: "FileUploads");

            migrationBuilder.DropTable(
                name: "HelpSubItems");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "InterviewAttendancesStatus");

            migrationBuilder.DropTable(
                name: "InterviewItemCandidatesFollowup");

            migrationBuilder.DropTable(
                name: "MessageComposeSources");

            migrationBuilder.DropTable(
                name: "MessageTypes");

            migrationBuilder.DropTable(
                name: "OrderItemAssessmentQs");

            migrationBuilder.DropTable(
                name: "PhoneMessages");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "ProspectiveCandidates");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "ReviewItemDatas");

            migrationBuilder.DropTable(
                name: "ReviewItems");

            migrationBuilder.DropTable(
                name: "ReviewItemStatuses");

            migrationBuilder.DropTable(
                name: "ReviewStatuses");

            migrationBuilder.DropTable(
                name: "SelectionStatuses");

            migrationBuilder.DropTable(
                name: "SkillDatas");

            migrationBuilder.DropTable(
                name: "SMSMessages");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "TaskTypes");

            migrationBuilder.DropTable(
                name: "UserAttachments");

            migrationBuilder.DropTable(
                name: "UserExps");

            migrationBuilder.DropTable(
                name: "UserHistoryItems");

            migrationBuilder.DropTable(
                name: "UserPassports");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserProfessions");

            migrationBuilder.DropTable(
                name: "UserQualifications");

            migrationBuilder.DropTable(
                name: "VoucherAttachments");

            migrationBuilder.DropTable(
                name: "VoucherEntries");

            migrationBuilder.DropTable(
                name: "AssessmentQBank");

            migrationBuilder.DropTable(
                name: "CandidateAssessments");

            migrationBuilder.DropTable(
                name: "ChecklistHRs");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "DLForwardCategories");

            migrationBuilder.DropTable(
                name: "SelectionDecisions");

            migrationBuilder.DropTable(
                name: "HelpItems");

            migrationBuilder.DropTable(
                name: "InterviewItemCandidates");

            migrationBuilder.DropTable(
                name: "OrderItemAssessments");

            migrationBuilder.DropTable(
                name: "ContractReviewItems");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.DropTable(
                name: "FinanceVouchers");

            migrationBuilder.DropTable(
                name: "DLForwardToAgents");

            migrationBuilder.DropTable(
                name: "Helps");

            migrationBuilder.DropTable(
                name: "InterviewItems");

            migrationBuilder.DropTable(
                name: "ContractReviews");

            migrationBuilder.DropTable(
                name: "UserHistoryHeaders");

            migrationBuilder.DropTable(
                name: "CustomerOfficials");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "CVRefs");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "JobDescriptions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Remunerations");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "DeployStages");
        }
    }
}
