using Employee.Application.Interfaces;
using Employee.Domain.Entities;

namespace Employee.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IKafkaProducer _kafkaProducer;

        public ClientService(IClientRepository clientRepository, IKafkaProducer kafkaProducer)
        {
            _clientRepository = clientRepository;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task CreateClientAsync(Client client)
        {
            await _clientRepository.AddAsync(client);
            await _kafkaProducer.ProduceAsync("client-registrations", $"New client registered: {client.Name} ({client.Email})");
        }

        public async Task UpdateClientAsync(Client client)
        {
            await _clientRepository.UpdateAsync(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }
    }
}
