using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.EntityFrameworkCore.Migrations;

public partial class AddMovementRequestWorkflowDefinition : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var workflowDefId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var step1Id = Guid.Parse("11111111-1111-1111-1111-111111111112");
        var step2Id = Guid.Parse("11111111-1111-1111-1111-111111111113");
        var step3Id = Guid.Parse("11111111-1111-1111-1111-111111111114");

        migrationBuilder.InsertData(
            table: "abp_workflow_definitions",
            columns: new[] { "Id", "Name", "Description", "IsActive", "CreationTime", "CreatorId" },
            values: new object[] { workflowDefId, "MovementRequest", "Malzeme hareketi onay akışı", true, DateTime.UtcNow, null });

        migrationBuilder.InsertData(
            table: "abp_workflow_step_definitions",
            columns: new[] { "Id", "WorkflowDefinitionId", "StepName", "StepOrder", "RequiredRoleName", "ResolverKey", "CreationTime", "CreatorId" },
            values: new object[,]
            {
                { step1Id, workflowDefId, "InitiatorManager", 1, null, "InitiatorManager", DateTime.UtcNow, null },
                { step2Id, workflowDefId, "TargetSiteManager", 2, null, "TargetSiteManager", DateTime.UtcNow, null },
                { step3Id, workflowDefId, "SourceSiteManager", 3, null, "SourceSiteManager", DateTime.UtcNow, null }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "abp_workflow_step_definitions",
            keyColumn: "Id",
            keyValue: Guid.Parse("11111111-1111-1111-1111-111111111112"));

        migrationBuilder.DeleteData(
            table: "abp_workflow_step_definitions",
            keyColumn: "Id",
            keyValue: Guid.Parse("11111111-1111-1111-1111-111111111113"));

        migrationBuilder.DeleteData(
            table: "abp_workflow_step_definitions",
            keyColumn: "Id",
            keyValue: Guid.Parse("11111111-1111-1111-1111-111111111114"));

        migrationBuilder.DeleteData(
            table: "abp_workflow_definitions",
            keyColumn: "Id",
            keyValue: Guid.Parse("11111111-1111-1111-1111-111111111111"));
    }
}
