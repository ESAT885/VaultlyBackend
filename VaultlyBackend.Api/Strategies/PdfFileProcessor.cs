using VaultlyBackend.Api.Strategies.Interfaces;

namespace VaultlyBackend.Api.Strategies
{
    public class PdfFileProcessor : IFileProcessor
    {
        public bool CanProcess(string contentType)
            => contentType == "application/pdf";

        public async Task ProcessAsync(string mergedPath, string storedFilefileName)
        {
            Console.WriteLine("PDF işleniyor...");
            // PDF metadata çıkarma
            await Task.CompletedTask;
        }
    }
}