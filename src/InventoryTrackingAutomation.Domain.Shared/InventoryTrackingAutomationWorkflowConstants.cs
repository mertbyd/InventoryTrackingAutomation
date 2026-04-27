namespace InventoryTrackingAutomation.Workflows;

// İş akışı tanım (WorkflowDefinition.Name) sabitleri — hardcoded string kullanımını engellemek için merkezi tanım.
public static class WorkflowDefinitionNames
{
    // MovementRequest entity'si için tanımlı iş akışı.
    public const string MovementRequest = "MovementRequest";
}

// İş akışına bağlanabilen entity tip adları (WorkflowInstance.EntityType).
public static class WorkflowEntityTypes
{
    // MovementRequest entity tipi.
    public const string MovementRequest = "MovementRequest";
}

// WorkflowStepDefinition.ResolverKey için kullanılan dinamik onaycı çözümleyici anahtarları.
public static class WorkflowResolverKeys
{
    // İş akışını başlatan kullanıcının doğrudan yöneticisini onaycı olarak çözer.
    public const string InitiatorManager = "InitiatorManager";

    // İş akışına bağlı entity'nin kaynak lokasyonu (Site) yöneticisini onaycı olarak çözer.
    public const string SourceSiteManager = "SourceSiteManager";

    // İş akışına bağlı entity'nin hedef lokasyonu (Site) yöneticisini onaycı olarak çözer.
    public const string TargetSiteManager = "TargetSiteManager";
}
