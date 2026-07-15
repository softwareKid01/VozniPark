using System.Collections.Generic;
using System.Threading.Tasks;
using VozniPark.Models;
using VozniPark.Repositories; 

namespace VozniPark.Services
{
    public class VoziloService : IVoziloService
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        
        public VoziloService(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<IEnumerable<Vozilo>> GetAllVozilaAsync()
        {
            
            return await _voziloRepository.GetAllAsync();
        }

        public async Task<Vozilo> GetVoziloByIdAsync(int id)
        {
            return await _voziloRepository.GetByIdAsync(id);
        }

        public async Task AddVoziloAsync(Vozilo vozilo)
        {
            await _voziloRepository.AddAsync(vozilo);
        }

        public async Task UpdateVoziloAsync(Vozilo vozilo)
        {
            await _voziloRepository.UpdateAsync(vozilo);
        }

        public async Task DeleteVoziloAsync(int id)
        {
            await _voziloRepository.DeleteAsync(id);
        }

        
        public async Task<Vozilo?> GetVoziloDetailsAsync(int id)
        {
            
            return await _voziloRepository.GetByIdWithIncludesAsync(id, v => v.Servisi);
        }

        public async Task<IEnumerable<Vozilo>> GetKriticneRegistracijeAsync()
        {
            
            var vozila = await _voziloRepository.GetAllAsync();

            
            return vozila.Where(v => v.RegistracijaIstekla || v.RegistracijaIsticeUskoro).ToList();
        }

        public async Task<IEnumerable<Vozilo>> GetKriticniServisiAsync()
        {
            
            var vozila = await _voziloRepository.GetAllWithIncludesAsync(v => v.Servisi);

            
            return vozila.Where(v => v.ServisIstekao || v.ServisIsticeUskoro).ToList();
        }


    }
}