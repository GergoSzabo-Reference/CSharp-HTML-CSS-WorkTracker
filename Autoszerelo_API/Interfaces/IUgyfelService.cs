using Autoszerelo.Shared;
using Autoszerelo_Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Autoszerelo_API.Interfaces
{
    public interface IUgyfelService
    {
        Task<IEnumerable<Ugyfel>> GetUgyfelekAsync();
        Task<IEnumerable<Munka>> GetMunkakByUgyfelIdAsync(int ugyfelId);
        Task<Ugyfel?> GetUgyfelByIdAsync(int id);
        Task<Ugyfel> CreateUgyfelAsync(Ugyfel ugyfel);
        Task<bool> UpdateUgyfelAsync(int id, Ugyfel ugyfel);
        Task<bool> DeleteUgyfelAsync(int id);
    }
}
