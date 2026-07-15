using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcosCLM.Migrations.SQL.Migrations
{
    /// <inheritdoc />
    public partial class CertificateRequest_UpdateParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiIdempotencyKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiIdempotencyKey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAuthority",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProviderType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AccountRef = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupportsAcme = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAuthority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CertificateType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "TLS_SERVER"),
                    KeyAlgorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "RSA"),
                    KeySize = table.Column<int>(type: "int", nullable: true),
                    CurveName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SignatureAlgorithm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValidityDays = table.Column<int>(type: "int", nullable: false),
                    RenewalWindowDays = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    SubjectTemplateJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    SanPolicyJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    RequireApproval = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CLMApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Criticality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "MEDIUM"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLMApplication", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentEnvironment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentEnvironment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventOutbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    Retries = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HsmCluster",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PartitionLabel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EndpointRef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FipsLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HsmCluster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagedDomain",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fqdn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValidationMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "DNS"),
                    ValidationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    ValidatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagedDomain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagedDomain_CLMApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "CLMApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentTarget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnvironmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EndpointRef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SecretRef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AutomationEnabled = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CLMApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeploymentTarget_CLMApplication_CLMApplicationId",
                        column: x => x.CLMApplicationId,
                        principalTable: "CLMApplication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeploymentTarget_DeploymentEnvironment_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "DeploymentEnvironment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HsmKeyRef",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HsmClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyLabel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    KeyHandle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KeySize = table.Column<int>(type: "int", nullable: true),
                    CurveName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Extractable = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ACTIVE"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HsmKeyRef", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HsmKeyRef_HsmCluster_HsmClusterId",
                        column: x => x.HsmClusterId,
                        principalTable: "HsmCluster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "DRAFT"),
                    CertificateRequestCLMApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateRequestDomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CertificateRequestProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HsmClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HsmKeyRefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubjectDn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KeyPolicyJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CsrPem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateRequest_CLMApplication_CertificateRequestCLMApplicationId",
                        column: x => x.CertificateRequestCLMApplicationId,
                        principalTable: "CLMApplication",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateRequest_CertificateAuthority_CaId",
                        column: x => x.CaId,
                        principalTable: "CertificateAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateRequest_CertificateProfile_CertificateRequestProfileId",
                        column: x => x.CertificateRequestProfileId,
                        principalTable: "CertificateProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateRequest_ManagedDomain_CertificateRequestDomainId",
                        column: x => x.CertificateRequestDomainId,
                        principalTable: "ManagedDomain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ApproverRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApproverUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    DecisionComment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DecidedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalTask_CertificateRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalOrderId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExternalCertificateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "CREATED"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RawResponseRef = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaOrder_CertificateAuthority_CaId",
                        column: x => x.CaId,
                        principalTable: "CertificateAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaOrder_CertificateRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DomainId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HsmKeyRefId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PreviousCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThumbprintSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SubjectDn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IssuerDn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NotBefore = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotAfter = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificatePem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChainPem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "ISSUED"),
                    RevocationReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_CertificateRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificate_Certificate_PreviousCertificateId",
                        column: x => x.PreviousCertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRequestSanDns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DnsName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequestSanDns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateRequestSanDns_CertificateRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRequestSanIp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateRequestSanIp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateRequestSanIp_CertificateRequest_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CertificateRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateDeployment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "PENDING"),
                    DeployedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateDeployment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateDeployment_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateDeployment_DeploymentTarget_TargetId",
                        column: x => x.TargetId,
                        principalTable: "DeploymentTarget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RenewalJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "SCHEDULED"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenewalJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RenewalJob_Certificate_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiIdempotencyKey_CustomerId_Key",
                table: "ApiIdempotencyKey",
                columns: new[] { "CustomerId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalTask_RequestId",
                table: "ApprovalTask",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CaOrder_CaId",
                table: "CaOrder",
                column: "CaId");

            migrationBuilder.CreateIndex(
                name: "IX_CaOrder_RequestId",
                table: "CaOrder",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_PreviousCertificateId",
                table: "Certificate",
                column: "PreviousCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_RequestId",
                table: "Certificate",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateDeployment_CertificateId",
                table: "CertificateDeployment",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateDeployment_TargetId",
                table: "CertificateDeployment",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequest_CaId",
                table: "CertificateRequest",
                column: "CaId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequest_CertificateRequestCLMApplicationId",
                table: "CertificateRequest",
                column: "CertificateRequestCLMApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequest_CertificateRequestDomainId",
                table: "CertificateRequest",
                column: "CertificateRequestDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequest_CertificateRequestProfileId",
                table: "CertificateRequest",
                column: "CertificateRequestProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequestSanDns_RequestId",
                table: "CertificateRequestSanDns",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateRequestSanIp_RequestId",
                table: "CertificateRequestSanIp",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentTarget_CLMApplicationId",
                table: "DeploymentTarget",
                column: "CLMApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentTarget_EnvironmentId",
                table: "DeploymentTarget",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HsmKeyRef_HsmClusterId",
                table: "HsmKeyRef",
                column: "HsmClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagedDomain_ApplicationId",
                table: "ManagedDomain",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_RenewalJob_CertificateId",
                table: "RenewalJob",
                column: "CertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiIdempotencyKey");

            migrationBuilder.DropTable(
                name: "ApprovalTask");

            migrationBuilder.DropTable(
                name: "CaOrder");

            migrationBuilder.DropTable(
                name: "CertificateDeployment");

            migrationBuilder.DropTable(
                name: "CertificateRequestSanDns");

            migrationBuilder.DropTable(
                name: "CertificateRequestSanIp");

            migrationBuilder.DropTable(
                name: "EventOutbox");

            migrationBuilder.DropTable(
                name: "HsmKeyRef");

            migrationBuilder.DropTable(
                name: "RenewalJob");

            migrationBuilder.DropTable(
                name: "DeploymentTarget");

            migrationBuilder.DropTable(
                name: "HsmCluster");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "DeploymentEnvironment");

            migrationBuilder.DropTable(
                name: "CertificateRequest");

            migrationBuilder.DropTable(
                name: "CertificateAuthority");

            migrationBuilder.DropTable(
                name: "CertificateProfile");

            migrationBuilder.DropTable(
                name: "ManagedDomain");

            migrationBuilder.DropTable(
                name: "CLMApplication");
        }
    }
}
