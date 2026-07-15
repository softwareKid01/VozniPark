using VozniPark.Models;

namespace VozniPark.Services
{
    public interface IServisService
    {
        Task<IEnumerable<Servis>> GetServisiByVoziloIdAsync(int voziloId);
        Task UpdateServisAsync(Servis servis);
       
        Task<Servis?> GetServisByIdAsync(int id);

    }
}
