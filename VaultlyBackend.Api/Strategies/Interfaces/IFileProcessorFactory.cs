namespace VaultlyBackend.Api.Strategies.Interfaces
{
    public interface IFileProcessorFactory
    {
       IFileProcessor GetProcessor(string contentType);
    }
}
