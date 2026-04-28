using Volo.Abp.Settings;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Settings;

public class InventoryTrackingAutomationSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        // İş akışı ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workflows.AllowedStates,
                $"{(int)WorkflowState.Active},{(int)WorkflowState.Completed},{(int)WorkflowState.Rejected}"
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workflows.AllowedActions,
                $"{(int)WorkflowActionType.Pending},{(int)WorkflowActionType.Approved},{(int)WorkflowActionType.Rejected}"
            )
        );

        // Ürün ve Birim ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Products.DefaultUnitType,
                ((int)UnitTypeEnum.Piece).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Products.AllowedUnitTypes,
                $"{(int)UnitTypeEnum.Piece},{(int)UnitTypeEnum.Kilogram},{(int)UnitTypeEnum.Liter},{(int)UnitTypeEnum.Meter}"
            )
        );

        // Araç ve Çalışan ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Vehicles.DefaultVehicleType,
                ((int)VehicleTypeEnum.Truck).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Vehicles.AllowedVehicleTypes,
                $"{(int)VehicleTypeEnum.Truck},{(int)VehicleTypeEnum.Van},{(int)VehicleTypeEnum.Car}"
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workers.DefaultWorkerType,
                ((int)WorkerTypeEnum.BlueCollar).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workers.AllowedWorkerTypes,
                $"{(int)WorkerTypeEnum.WhiteCollar},{(int)WorkerTypeEnum.BlueCollar},{(int)WorkerTypeEnum.Subcontractor}"
            )
        );

        // Hareket Talebi ve Onay ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.MovementRequests.DefaultStatus,
                ((int)MovementStatusEnum.Pending).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.MovementRequests.AllowedStatuses,
                $"{(int)MovementStatusEnum.Pending},{(int)MovementStatusEnum.InReview},{(int)MovementStatusEnum.Approved},{(int)MovementStatusEnum.Shipped},{(int)MovementStatusEnum.Completed},{(int)MovementStatusEnum.Cancelled},{(int)MovementStatusEnum.Rejected}"
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.MovementRequests.DefaultPriority,
                ((int)MovementPriorityEnum.Normal).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.MovementRequests.AllowedPriorities,
                $"{(int)MovementPriorityEnum.Low},{(int)MovementPriorityEnum.Normal},{(int)MovementPriorityEnum.High},{(int)MovementPriorityEnum.Urgent}"
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Movements.AllowedApprovalStatuses,
                $"{(int)ApprovalStatusEnum.Pending},{(int)ApprovalStatusEnum.Approved},{(int)ApprovalStatusEnum.Rejected}"
            )
        );

        // Envanter İşlem ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.InventoryTransactions.DefaultType,
                ((int)InventoryTransactionTypeEnum.WarehouseToVehicle).ToString()
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.InventoryTransactions.AllowedTypes,
                $"{(int)InventoryTransactionTypeEnum.WarehouseToVehicle},{(int)InventoryTransactionTypeEnum.VehicleToWarehouse},{(int)InventoryTransactionTypeEnum.WarehouseToWarehouse},{(int)InventoryTransactionTypeEnum.Adjustment}"
            )
        );
    }
}
