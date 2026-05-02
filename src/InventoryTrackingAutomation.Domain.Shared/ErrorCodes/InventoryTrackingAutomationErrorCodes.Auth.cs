namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class Auth
    {
        public const string UserNameAlreadyExists = Prefix + ":Auth.UserNameAlreadyExists";
        public const string EmailAlreadyExists = Prefix + ":Auth.EmailAlreadyExists";
        public const string InvalidCredentials = Prefix + ":Auth.InvalidCredentials";
        public const string PasswordMismatch = Prefix + ":Auth.PasswordMismatch";
        public const string UserCreationFailed = Prefix + ":Auth.UserCreationFailed";
        public const string RoleNotFound = Prefix + ":Auth.RoleNotFound";
        public const string RoleAssignmentFailed = Prefix + ":Auth.RoleAssignmentFailed";
        public const string TokenRequestFailed = Prefix + ":Auth.TokenRequestFailed";
        public const string MissingConfiguration = Prefix + ":Auth.MissingConfiguration";
    }
}
