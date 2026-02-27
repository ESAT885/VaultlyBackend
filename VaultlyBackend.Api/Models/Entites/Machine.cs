namespace VaultlyBackend.Api.Models.Entites
{
    public class Machine
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string InstallDate { get; set; }
        public MachineStatus? Status { get; set; }

    }
    public enum MachineStatus
    {
        Active,
        Inactive,
        UnderMaintenance
    }
}
