using Employee.Application.Interfaces;
using Employee.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Employee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return Ok(new { message = "Clients retrieved successfully.", data = clients });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null) 
            {
                return NotFound(new { message = "Client not found." });
            }
            return Ok(new { message = "Client retrieved successfully.", data = client });
        }

        [HttpPost]
        public async Task<ActionResult> Create(Client client)
        {
            await _clientService.CreateClientAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, new { message = "Client created successfully.", data = client });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Client client)
        {
            if (id != client.Id) return BadRequest(new { message = "ID mismatch." });

            var existingClient = await _clientService.GetClientByIdAsync(id);
            if (existingClient == null)
            {
                return NotFound(new { message = "Client not found." });
            }

            await _clientService.UpdateClientAsync(client);
            return Ok(new { message = "Client updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingClient = await _clientService.GetClientByIdAsync(id);
            if (existingClient == null)
            {
                return NotFound(new { message = "Client not found." });
            }

            await _clientService.DeleteClientAsync(id);
            return Ok(new { message = "Client deleted successfully." });
        }
    }
}
