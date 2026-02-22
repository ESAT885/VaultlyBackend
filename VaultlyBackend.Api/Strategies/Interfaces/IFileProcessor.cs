using Microsoft.AspNetCore.Mvc.Filters;

namespace VaultlyBackend.Api.Strategies.Interfaces
{
    public interface IFileProcessor
    {
        bool CanProcess(string contentType);
        Task ProcessAsync(string mergedPath,  string storedFilefileName);
    }
}
