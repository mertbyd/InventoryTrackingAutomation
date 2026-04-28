using System;
using System.Linq;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using Volo.Abp.Settings;

namespace InventoryTrackingAutomation.Settings;

public class InventoryTrackingAutomationSettingDefinitionProvider : SettingDefinitionProvider
{
    private string GetEnumValues<TEnum>() where TEnum : Enum
    {
        return string.Join(",", Enum.GetValues(typeof(TEnum)).Cast<object>().Select(Convert.ToInt32));
    }

    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(new SettingDefinition(
            InventoryTrackingAutomationSettings.Workflows.AllowedStates,
            GetEnumValues<WorkflowState>()
        ));

        context.Add(new SettingDefinition(
            InventoryTrackingAutomationSettings.Workflows.AllowedActions,
            GetEnumValues<WorkflowActionType>()
        ));

        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Masters.AllowedUnitTypes, GetEnumValues<UnitTypeEnum>()));
        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes, GetEnumValues<WorkerTypeEnum>()));
        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Masters.AllowedSiteTypes, GetEnumValues<SiteTypeEnum>()));
        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Masters.AllowedVehicleTypes, GetEnumValues<VehicleTypeEnum>()));

        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities, GetEnumValues<MovementPriorityEnum>()));
        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses, GetEnumValues<MovementStatusEnum>()));
        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Movements.AllowedApprovalStatuses, GetEnumValues<ApprovalStatusEnum>()));

        context.Add(new SettingDefinition(InventoryTrackingAutomationSettings.Stock.AllowedStockMovementTypes, GetEnumValues<StockMovementTypeEnum>()));

    }
}
