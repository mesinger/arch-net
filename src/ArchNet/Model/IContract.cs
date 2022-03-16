namespace ArchNet.Model;

public interface IContract
{
    // where am i?
    Module Module();
        
    // what can i do for you?
    IEnumerable<ExposedCapability> Capabilities();
    bool Exposes(ExposedCapability capability);
    bool ExposesExactly(params ExposedCapability[] capabilities);
        
    // what do i need?
    IEnumerable<Usage> Usages();
    bool UsesContract(IContract contract);
    bool UsesModule(Module module);
    bool UsesContractNot(IContract contract);
    bool UsesModuleNot(Module module);
    bool UsesExactlyTheseContracts(params IContract[] contracts);
    bool UsesExactlyTheseModules(params Module[] modules);

    // Who uses me?
}