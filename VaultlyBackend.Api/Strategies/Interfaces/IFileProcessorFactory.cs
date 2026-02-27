namespace VaultlyBackend.Api.Strategies.Interfaces
{
    public interface IFileProcessorFactory
    {
        IEnumerable<IFileProcessor> GetProcessors(string contentType);
    }
}
