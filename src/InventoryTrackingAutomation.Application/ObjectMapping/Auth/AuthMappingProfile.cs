using AutoMapper;
using InventoryTrackingAutomation.Dtos.Auth;
using InventoryTrackingAutomation.Models.Auth;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Auth;

/// <summary>
/// Auth DTO'ları ile Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // Login DTO → Login Model
        CreateMap<LoginDto, LoginModel>();

        // Register DTO → Register Model
        CreateMap<RegisterDto, RegisterModel>();
    }
}
