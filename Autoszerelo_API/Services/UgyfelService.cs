using Autoszerelo.Shared;
using Autoszerelo_API.Data;
using Autoszerelo_API.Interfaces;
using Autoszerelo_Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autoszerelo_API.Services
{
    public class UgyfelService : IUgyfelService
    {
        private readonly AutoszereloDbContext _context;
        private readonly WorkHourEstimationService _estimationService;

        public UgyfelService(AutoszereloDbContext context, WorkHourEstimationService estimationService)
        {
            _context = context;
            _estimationService = estimationService;
        }

        public async Task<IEnumerable<Ugyfel>> GetUgyfelekAsync()
        {
            return await _context.Ugyfelek.ToListAsync();
        }

        public async Task<IEnumerable<Munka>> GetMunkakByUgyfelIdAsync(int ugyfelId)
        {
            var munkak = await _context.Munkak
                .Where(m => m.UgyfelId == ugyfelId)
                .ToListAsync();

            foreach(var munka in munkak)
            {
                munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);
            }
            return munkak;
        }

        public async Task<Ugyfel?> GetUgyfelByIdAsync(int id)
        {
            return await _context.Ugyfelek.FindAsync(id);
        }

        public async Task<Ugyfel> CreateUgyfelAsync(Ugyfel ugyfel)
        {
            ugyfel.UgyfelId = 0;
            _context.Ugyfelek.Add(ugyfel);
            await _context.SaveChangesAsync();
            return ugyfel;
        }

        public async Task<bool> UpdateUgyfelAsync(int id, Ugyfel ugyfel)
        {
            if(id != ugyfel.UgyfelId)
            {
                return false;
            }

            _context.Entry(ugyfel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!await UgyfelExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteUgyfelAsync(int id)
        {
            var ugyfel = await _context.Ugyfelek.FindAsync(id);

            if(ugyfel == null)
            {
                return false;
            }

            _context.Ugyfelek.Remove(ugyfel);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> UgyfelExistsAsync(int id)
        {
            return await _context.Ugyfelek.AnyAsync(u => u.UgyfelId == id);
        }
    }
}
