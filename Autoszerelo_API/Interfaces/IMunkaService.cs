using Autoszerelo.Shared;
using Autoszerelo_Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autoszerelo_API.Interfaces
{
    public interface IMunkaService
    {
        Task<IEnumerable<Munka>> GetMunkakAsync();
        Task<Munka?> GetMunkaByIdAsync(int id);
        Task<Munka> CreateMunkaAsync(Munka munka);
        Task<bool> UpdateMunkaAsync(int id, Munka munka);
        Task<bool> DeleteMunkaAsync(int id);
        // Task<bool> UpdateMunkaStatusAsync(int id, MunkaAllapot ujAllapot);
    }
}