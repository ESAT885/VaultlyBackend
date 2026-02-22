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

        public IFileProcessor GetProcessor(string contentType)
        {
            var processor = _processors
                .FirstOrDefault(p => p.CanProcess(contentType));

            if (processor == null)
                throw new NotSupportedException("Desteklenmeyen dosya tipi");

            return processor;
        }
    }
}
