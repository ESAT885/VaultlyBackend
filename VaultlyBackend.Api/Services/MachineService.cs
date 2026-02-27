using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Models.Dtos;
using VaultlyBackend.Api.Models.Entites;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Services
{
    public class MachineService(VaultlyDbContext context, IMapper mapper) : IMachineService
    {
        public async Task<List<MachineDto>> GetMachines()
        {
            var list = await context.Machines.AsNoTracking().ToListAsync();
            return mapper.Map<List<MachineDto>>(list);
        }

        public async Task<MachineDto?> GetMachine(Guid id)
        {
            var entity = await context.Machines.FindAsync(id);
            return entity is null ? null : mapper.Map<MachineDto>(entity);
        }

        public async Task<MachineDto> CreateMachine(MachineDto request)
        {
            var entity = new Machine
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                InstallDate = request.InstallDate,
                Model = request.Model,
                SerialNumber = request.SerialNumber,
                //Status = request.Status    ,
                Status = MachineStatus.Active,
            };

            context.Machines.Add(entity);
            await context.SaveChangesAsync();

            return mapper.Map<MachineDto>(entity);
        }

        public async Task<MachineDto?> UpdateMachine(Guid id, MachineDto request)
        {
            var entity = await context.Machines.FindAsync(id);
            if (entity is null)
                return null;

            entity.Name = request.Name;
         

            context.Machines.Update(entity);
            await context.SaveChangesAsync();

            return mapper.Map<MachineDto>(entity);
        }

        public async Task<bool> DeleteMachine(Guid id)
        {
            var entity = await context.Machines.FindAsync(id);
            if (entity is null)
                return false;

            context.Machines.Remove(entity);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
