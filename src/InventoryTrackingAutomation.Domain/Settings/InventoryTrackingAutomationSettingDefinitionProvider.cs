using Volo.Abp.Settings;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Constants.Lookups;

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
                UnitTypeCodes.Piece
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Products.AllowedUnitTypes,
                string.Join(",", UnitTypeCodes.Piece, UnitTypeCodes.Box, UnitTypeCodes.Kg, UnitTypeCodes.Meter, UnitTypeCodes.Liter)
            )
        );

        // Araç ve Çalışan ayarları
        context.Add(
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Vehicles.DefaultVehicleType,
                VehicleTypeCodes.Truck
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Vehicles.AllowedVehicleTypes,
                string.Join(",", VehicleTypeCodes.Truck, VehicleTypeCodes.Van, VehicleTypeCodes.Car)
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workers.DefaultWorkerType,
                WorkerTypeCodes.BlueCollar
            ),
            new SettingDefinition(
                InventoryTrackingAutomationSettings.Workers.AllowedWorkerTypes,
                string.Join(",", WorkerTypeCodes.WhiteCollar, WorkerTypeCodes.BlueCollar, WorkerTypeCodes.Subcontractor)
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
