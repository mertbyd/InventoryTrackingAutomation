START TRANSACTION;
DROP INDEX abp."IX_AbpPermissions_Name";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "CreationTime";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "CreatorId";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "DeleterId";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "DeletionTime";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "IsDeleted";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "LastModificationTime";

ALTER TABLE openiddict."OpenIddictTokens" DROP COLUMN "LastModifierId";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "CreationTime";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "CreatorId";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "DeleterId";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "DeletionTime";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "IsDeleted";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "LastModificationTime";

ALTER TABLE openiddict."OpenIddictAuthorizations" DROP COLUMN "LastModifierId";

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'workflow') THEN
        CREATE SCHEMA workflow;
    END IF;
END $EF$;

ALTER TABLE "WorkflowStepDefinitions" SET SCHEMA workflow;

ALTER TABLE "WorkflowInstanceSteps" SET SCHEMA workflow;

ALTER TABLE "WorkflowInstances" SET SCHEMA workflow;

ALTER TABLE "WorkflowDefinitions" SET SCHEMA workflow;

ALTER TABLE "OpenIddictApplications" RENAME COLUMN "Type" TO "ClientType";

ALTER TABLE master."Workers" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE master."Workers" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE master."Workers" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE master."Vehicles" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE master."Vehicles" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE master."Vehicles" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE stock."StockMovements" ALTER COLUMN "OccurredAt" TYPE timestamp with time zone;

ALTER TABLE stock."StockMovements" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE stock."StockMovements" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE stock."StockMovements" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE master."Sites" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE master."Sites" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE master."Sites" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "PlannedDepartureTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "DepartureTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "DeliveryTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE shipment."Shipments" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE shipment."ShipmentLines" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE shipment."ShipmentLines" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE shipment."ShipmentLines" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE stock."ProductStocks" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE stock."ProductStocks" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE stock."ProductStocks" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE master."Products" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE master."Products" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE master."Products" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE lookup."ProductCategories" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE lookup."ProductCategories" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE lookup."ProductCategories" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementRequests" ALTER COLUMN "PlannedDate" TYPE timestamp with time zone;

ALTER TABLE movement."MovementRequests" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE movement."MovementRequests" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE movement."MovementRequests" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE movement."MovementRequests" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE movement."MovementRequests" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementRequests" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE movement."MovementRequests" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE movement."MovementRequests" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE movement."MovementRequests" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE movement."MovementRequestLines" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementRequestLines" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementRequestLines" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementApprovals" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE movement."MovementApprovals" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE movement."MovementApprovals" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE movement."MovementApprovals" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE movement."MovementApprovals" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE movement."MovementApprovals" ALTER COLUMN "DecidedAt" TYPE timestamp with time zone;

ALTER TABLE movement."MovementApprovals" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE movement."MovementApprovals" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE movement."MovementApprovals" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE movement."MovementApprovals" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE lookup."Departments" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE lookup."Departments" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE lookup."Departments" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowStepDefinitions" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowStepDefinitions" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowInstanceSteps" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowInstanceSteps" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowInstanceSteps" ALTER COLUMN "ActionDate" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE workflow."WorkflowInstances" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE workflow."WorkflowInstances" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE workflow."WorkflowInstances" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE workflow."WorkflowDefinitions" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE workflow."WorkflowDefinitions" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE workflow."WorkflowDefinitions" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "OpenIddictTokens" ALTER COLUMN "Type" TYPE character varying(150);

ALTER TABLE "OpenIddictTokens" ALTER COLUMN "RedemptionDate" TYPE timestamp with time zone;

UPDATE "OpenIddictTokens" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "OpenIddictTokens" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "OpenIddictTokens" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "OpenIddictTokens" ALTER COLUMN "ExpirationDate" TYPE timestamp with time zone;

ALTER TABLE "OpenIddictTokens" ALTER COLUMN "CreationDate" TYPE timestamp with time zone;

UPDATE "OpenIddictTokens" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "OpenIddictTokens" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "OpenIddictTokens" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "OpenIddictScopes" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE "OpenIddictScopes" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "OpenIddictScopes" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "OpenIddictScopes" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "OpenIddictScopes" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE "OpenIddictScopes" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "OpenIddictScopes" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "OpenIddictScopes" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "OpenIddictScopes" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

UPDATE "OpenIddictAuthorizations" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "OpenIddictAuthorizations" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "OpenIddictAuthorizations" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "OpenIddictAuthorizations" ALTER COLUMN "CreationDate" TYPE timestamp with time zone;

UPDATE "OpenIddictAuthorizations" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "OpenIddictAuthorizations" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "OpenIddictAuthorizations" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "OpenIddictApplications" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE "OpenIddictApplications" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "OpenIddictApplications" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "OpenIddictApplications" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "OpenIddictApplications" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE "OpenIddictApplications" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "OpenIddictApplications" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "OpenIddictApplications" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "OpenIddictApplications" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "OpenIddictApplications" ADD "ApplicationType" character varying(50);

ALTER TABLE "OpenIddictApplications" ADD "FrontChannelLogoutUri" text;

ALTER TABLE "OpenIddictApplications" ADD "JsonWebKeySet" text;

ALTER TABLE "OpenIddictApplications" ADD "Settings" text;

ALTER TABLE "AbpUsers" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE "AbpUsers" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpUsers" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpUsers" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "AbpUsers" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE "AbpUsers" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "AbpUsers" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpUsers" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpUsers" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpUsers" ADD "EntityVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE "AbpUsers" ADD "LastPasswordChangeTime" timestamp with time zone;

ALTER TABLE "AbpUsers" ADD "LastSignInTime" timestamp with time zone;

ALTER TABLE "AbpUsers" ADD "Leaved" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "AbpUsers" ADD "ShouldChangePasswordOnNextLogin" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "AbpUserOrganizationUnits" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE "AbpTenants" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE "AbpTenants" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpTenants" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpTenants" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "AbpTenants" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE "AbpTenants" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "AbpTenants" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpTenants" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpTenants" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpTenants" ADD "EntityVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE "AbpTenants" ADD "NormalizedName" character varying(64) NOT NULL DEFAULT '';

UPDATE "AbpSecurityLogs" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpSecurityLogs" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpSecurityLogs" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "AbpSecurityLogs" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "AbpSecurityLogs" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpSecurityLogs" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpSecurityLogs" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

UPDATE "AbpRoles" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpRoles" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpRoles" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

UPDATE "AbpRoles" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpRoles" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpRoles" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpRoles" ADD "CreationTime" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "AbpRoles" ADD "EntityVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE "AbpPermissions" ALTER COLUMN "GroupName" DROP NOT NULL;

ALTER TABLE "AbpPermissions" ADD "ManagementPermissionName" character varying(128);

ALTER TABLE "AbpPermissions" ADD "ResourceName" character varying(256);

ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "LastModificationTime" TYPE timestamp with time zone;

UPDATE "AbpOrganizationUnits" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "DeletionTime" TYPE timestamp with time zone;

ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

UPDATE "AbpOrganizationUnits" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpOrganizationUnits" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpOrganizationUnits" ADD "EntityVersion" integer NOT NULL DEFAULT 0;

ALTER TABLE "AbpOrganizationUnitRoles" ALTER COLUMN "CreationTime" TYPE timestamp with time zone;

ALTER TABLE "AbpEntityPropertyChanges" ALTER COLUMN "PropertyTypeFullName" TYPE character varying(512);

ALTER TABLE "AbpEntityChanges" ALTER COLUMN "EntityTypeFullName" TYPE character varying(512);

ALTER TABLE "AbpEntityChanges" ALTER COLUMN "EntityId" DROP NOT NULL;

ALTER TABLE "AbpEntityChanges" ALTER COLUMN "ChangeTime" TYPE timestamp with time zone;

UPDATE "AbpClaimTypes" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpClaimTypes" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpClaimTypes" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

UPDATE "AbpClaimTypes" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpClaimTypes" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpClaimTypes" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpClaimTypes" ADD "CreationTime" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

UPDATE "AbpAuditLogs" SET "ExtraProperties" = '' WHERE "ExtraProperties" IS NULL;
ALTER TABLE "AbpAuditLogs" ALTER COLUMN "ExtraProperties" SET NOT NULL;
ALTER TABLE "AbpAuditLogs" ALTER COLUMN "ExtraProperties" SET DEFAULT '';

ALTER TABLE "AbpAuditLogs" ALTER COLUMN "ExecutionTime" TYPE timestamp with time zone;

UPDATE "AbpAuditLogs" SET "ConcurrencyStamp" = '' WHERE "ConcurrencyStamp" IS NULL;
ALTER TABLE "AbpAuditLogs" ALTER COLUMN "ConcurrencyStamp" SET NOT NULL;
ALTER TABLE "AbpAuditLogs" ALTER COLUMN "ConcurrencyStamp" SET DEFAULT '';

ALTER TABLE "AbpAuditLogActions" ALTER COLUMN "ExecutionTime" TYPE timestamp with time zone;

CREATE TABLE "AbpAuditLogExcelFiles" (
    "Id" uuid NOT NULL,
    "TenantId" uuid,
    "FileName" character varying(256),
    "CreationTime" timestamp with time zone NOT NULL,
    "CreatorId" uuid,
    CONSTRAINT "PK_AbpAuditLogExcelFiles" PRIMARY KEY ("Id")
);

CREATE TABLE "AbpResourcePermissionGrants" (
    "Id" uuid NOT NULL,
    "TenantId" uuid,
    "Name" character varying(128) NOT NULL,
    "ProviderName" character varying(64) NOT NULL,
    "ProviderKey" character varying(64) NOT NULL,
    "ResourceName" character varying(256) NOT NULL,
    "ResourceKey" character varying(256) NOT NULL,
    CONSTRAINT "PK_AbpResourcePermissionGrants" PRIMARY KEY ("Id")
);

CREATE TABLE "AbpSessions" (
    "Id" uuid NOT NULL,
    "SessionId" character varying(128) NOT NULL,
    "Device" character varying(64) NOT NULL,
    "DeviceInfo" character varying(256),
    "TenantId" uuid,
    "UserId" uuid NOT NULL,
    "ClientId" character varying(64),
    "IpAddresses" character varying(2048),
    "SignedIn" timestamp with time zone NOT NULL,
    "LastAccessed" timestamp with time zone,
    "ExtraProperties" text,
    CONSTRAINT "PK_AbpSessions" PRIMARY KEY ("Id")
);

CREATE TABLE "AbpSettingDefinitions" (
    "Id" uuid NOT NULL,
    "Name" character varying(128) NOT NULL,
    "DisplayName" character varying(256) NOT NULL,
    "Description" character varying(512),
    "DefaultValue" character varying(2048),
    "IsVisibleToClients" boolean NOT NULL,
    "Providers" character varying(1024),
    "IsInherited" boolean NOT NULL,
    "IsEncrypted" boolean NOT NULL,
    "ExtraProperties" text,
    CONSTRAINT "PK_AbpSettingDefinitions" PRIMARY KEY ("Id")
);

CREATE TABLE "AbpUserDelegations" (
    "Id" uuid NOT NULL,
    "TenantId" uuid,
    "SourceUserId" uuid NOT NULL,
    "TargetUserId" uuid NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    "EndTime" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_AbpUserDelegations" PRIMARY KEY ("Id")
);

CREATE TABLE "AbpUserPasskeys" (
    "CredentialId" bytea NOT NULL,
    "TenantId" uuid,
    "UserId" uuid NOT NULL,
    "Data" jsonb,
    CONSTRAINT "PK_AbpUserPasskeys" PRIMARY KEY ("CredentialId"),
    CONSTRAINT "FK_AbpUserPasskeys_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AbpUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AbpUserPasswordHistories" (
    "UserId" uuid NOT NULL,
    "Password" character varying(256) NOT NULL,
    "TenantId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_AbpUserPasswordHistories" PRIMARY KEY ("UserId", "Password"),
    CONSTRAINT "FK_AbpUserPasswordHistories_AbpUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AbpUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AbpTenants_NormalizedName" ON "AbpTenants" ("NormalizedName");

CREATE UNIQUE INDEX "IX_AbpPermissions_ResourceName_Name" ON "AbpPermissions" ("ResourceName", "Name");

CREATE UNIQUE INDEX "IX_AbpResourcePermissionGrants_TenantId_Name_ResourceName_Reso~" ON "AbpResourcePermissionGrants" ("TenantId", "Name", "ResourceName", "ResourceKey", "ProviderName", "ProviderKey");

CREATE INDEX "IX_AbpSessions_Device" ON "AbpSessions" ("Device");

CREATE INDEX "IX_AbpSessions_SessionId" ON "AbpSessions" ("SessionId");

CREATE INDEX "IX_AbpSessions_TenantId_UserId" ON "AbpSessions" ("TenantId", "UserId");

CREATE UNIQUE INDEX "IX_AbpSettingDefinitions_Name" ON "AbpSettingDefinitions" ("Name");

CREATE INDEX "IX_AbpUserPasskeys_UserId" ON "AbpUserPasskeys" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260424171155_Upgrade_To_ABP_10_3_Host', '10.0.2');

COMMIT;

