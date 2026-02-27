using AutoMapper;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Models.Dtos;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Services.Interfaces
{
    public interface IMachineService
    {
        Task<List<MachineDto>> GetMachines();
        Task<MachineDto?> GetMachine(Guid id);
        public  Task<MachineDto> CreateMachine(MachineDto request);
        public  Task<MachineDto?> UpdateMachine(Guid id, MachineDto request);
        Task<bool> DeleteMachine(Guid id);
    }
}

