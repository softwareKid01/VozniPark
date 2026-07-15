using VozniPark.Models;

namespace VozniPark.Services
{
    public interface IVoziloService
    {
        Task<IEnumerable<Vozilo>> GetAllVozilaAsync();
        Task<Vozilo?> GetVoziloByIdAsync(int id);
        Task AddVoziloAsync(Vozilo vozilo);
        Task UpdateVoziloAsync(Vozilo vozilo);
        Task DeleteVoziloAsync(int id);
        Task<Vozilo?> GetVoziloDetailsAsync(int id);
        
        Task<IEnumerable<Vozilo>> GetKriticneRegistracijeAsync();
        Task<IEnumerable<Vozilo>> GetKriticniServisiAsync();
    }
}
