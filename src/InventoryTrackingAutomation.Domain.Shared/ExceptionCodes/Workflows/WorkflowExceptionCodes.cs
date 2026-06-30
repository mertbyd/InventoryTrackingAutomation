namespace InventoryTrackingAutomation.ExceptionCodes;

public static class WorkflowExceptionCodes
{
    private const string WorkflowErrorCodesPrefix = $"WorkflowManagement.Workflow";
    public const string DefinitionNotFound = $"{WorkflowErrorCodesPrefix}:00001";
    public const string InstanceNotFound = $"{WorkflowErrorCodesPrefix}:00002";
    public const string StepNotFound = $"{WorkflowErrorCodesPrefix}:00003";
    public const string UnauthorizedApproval = $"{WorkflowErrorCodesPrefix}:00004";
    public const string InstanceNotActive = $"{WorkflowErrorCodesPrefix}:00005";

    public static class ValidationExceptions
    {
        private const string WorkflowValidationExceptionsPrefix = $"{WorkflowErrorCodesPrefix}.ValidationExceptions";

        public static class Definition
        {
            public const string NameCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.DefinitionName:00001";
            public const string NameMaxLength = $"{WorkflowValidationExceptionsPrefix}.DefinitionName:00002";
            public const string VersionInvalid = $"{WorkflowValidationExceptionsPrefix}.Version:00001";
        }

        public static class StepDefinition
        {
            public const string OrderInvalid = $"{WorkflowValidationExceptionsPrefix}.Order:00001";
            public const string RoleNameCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.RoleName:00001";
            public const string RoleNameMaxLength = $"{WorkflowValidationExceptionsPrefix}.RoleName:00002";
            public const string ResolverKeyMaxLength = $"{WorkflowValidationExceptionsPrefix}.ResolverKey:00001";
        }

        public static class ProcessApproval
        {
            public const string StepIdCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.StepId:00001";
            public const string StepIdInvalid = $"{WorkflowValidationExceptionsPrefix}.StepId:00002";
            public const string NoteMaxLength = $"{WorkflowValidationExceptionsPrefix}.Note:00001";
        }

        public static class StartWorkflow
        {
            public const string EntityTypeCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.EntityType:00001";
            public const string EntityTypeMaxLength = $"{WorkflowValidationExceptionsPrefix}.EntityType:00002";
            public const string EntityIdCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.EntityId:00001";
            public const string EntityIdInvalid = $"{WorkflowValidationExceptionsPrefix}.EntityId:00002";
            public const string DefinitionIdCannotEmpty = $"{WorkflowValidationExceptionsPrefix}.DefinitionId:00001";
            public const string DefinitionIdInvalid = $"{WorkflowValidationExceptionsPrefix}.DefinitionId:00002";
        }
    }
}
