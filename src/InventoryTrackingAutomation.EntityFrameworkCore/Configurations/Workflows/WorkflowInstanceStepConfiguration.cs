using InventoryTrackingAutomation.Entities.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Workflows;

/// <summary>
/// WorkflowInstanceStep entity'si için veritabanı şema konfigürasyonu.
/// </summary>
public class WorkflowInstanceStepConfiguration : IEntityTypeConfiguration<WorkflowInstanceStep>
{
    public void Configure(EntityTypeBuilder<WorkflowInstanceStep> builder)
    {
        builder.ToTable(InventoryTrackingAutomationDbProperties.DbTablePrefix + "WorkflowInstanceSteps", InventoryTrackingAutomationDbProperties.WorkflowSchema);
        
        builder.ConfigureByConvention(); // ABP'nin standart audit kolonlarını ekler

        builder.Property(x => x.Note).HasMaxLength(500);

        // StepDefinition ilişkisi (Restrict Delete)
        builder.HasOne(x => x.WorkflowStepDefinition)
            .WithMany()
            .HasForeignKey(x => x.WorkflowStepDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
