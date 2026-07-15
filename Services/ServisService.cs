using VozniPark.Models;
using VozniPark.Repositories;

namespace VozniPark.Services
{
    public class ServisService : IServisService
    {
        private readonly IRepository<Servis> _servisRepository;
        public ServisService(IRepository<Servis> voziloRepository)
        {
            _servisRepository = voziloRepository;
        }
        public async Task<IEnumerable<Servis>> GetServisiByVoziloIdAsync(int voziloId)
        {
            
            return await _servisRepository.GetAllAsync(s => s.VoziloId == voziloId);
        }

        public async Task UpdateServisAsync(Servis servis)
        {
            await _servisRepository.UpdateAsync(servis);
        }

        
        public async Task<Servis?> GetServisByIdAsync(int id)
        {
            
            return await _servisRepository.GetByIdAsync(id);
        }


    }
}
