using VaultlyBackend.Api.Models.Entites;

namespace VaultlyBackend.Api.Models.Dtos
{
    public class MachineDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string InstallDate { get; set; }
       
    }
}
