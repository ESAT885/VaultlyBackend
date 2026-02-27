using VaultlyBackend.Api.Strategies.Interfaces;

namespace VaultlyBackend.Api.Strategies
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IEnumerable<IFileProcessor> _processors;

        public FileProcessorFactory(IEnumerable<IFileProcessor> processors)
        {
            _processors = processors;
        }

        public IEnumerable<IFileProcessor> GetProcessors(string contentType)
        {
            var processors = _processors
                .Where(p => p.CanProcess(contentType)).ToList();

            if (processors == null)
                throw new NotSupportedException("Desteklenmeyen dosya tipi");

            return processors;
        }
    }
}
