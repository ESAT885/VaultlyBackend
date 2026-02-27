using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Models.Dtos;
using VaultlyBackend.Api.Models.Dtos.StoredFiles;
using VaultlyBackend.Api.Services;
using VaultlyBackend.Api.Services.Interfaces;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinesController(IMachineService machineService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetMachines()
        {
            var result = await machineService.GetMachines();
            return Success<List<MachineDto>>(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMachine(Guid id)
        {
            var result = await machineService.GetMachine(id);
            if (result is null)
                return NotFound();
            return Success<MachineDto>(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMachine([FromBody] MachineDto request)
        {
            var created = await machineService.CreateMachine(request);
            return Success<MachineDto>(created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMachine(Guid id, [FromBody] MachineDto request)
        {
            var updated = await machineService.UpdateMachine(id, request);
            if (updated is null)
                return NotFound();
            return Success<MachineDto>(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMachine(Guid id)
        {
            var deleted = await machineService.DeleteMachine(id);
            if (!deleted)
                return NotFound();
            return Success("Machine deleted");
        }
    }
}
