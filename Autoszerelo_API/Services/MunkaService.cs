// Autoszerelo_API/Services/MunkaService.cs
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
    public class MunkaService : IMunkaService
    {
        private readonly AutoszereloDbContext _context;
        private readonly WorkHourEstimationService _estimationService;
        private readonly IUgyfelService _ugyfelService;

        public MunkaService(AutoszereloDbContext context,
                            WorkHourEstimationService estimationService,
                            IUgyfelService ugyfelService)
        {
            _context = context;
            _estimationService = estimationService;
            _ugyfelService = ugyfelService;
        }

        public async Task<IEnumerable<Munka>> GetMunkakAsync()
        {
            var munkak = await _context.Munkak.ToListAsync();
            foreach (var munka in munkak)
            {
                munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);
            }
            return munkak;
        }

        public async Task<Munka?> GetMunkaByIdAsync(int id)
        {
            var munka = await _context.Munkak.FindAsync(id);
            if (munka != null)
            {
                munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);
            }
            return munka;
        }

        public async Task<Munka> CreateMunkaAsync(Munka munka)
        {
            var ugyfel = await _ugyfelService.GetUgyfelByIdAsync(munka.UgyfelId);
            if (ugyfel == null)
            {
                throw new ArgumentException($"Nem létezik ügyfél a megadott UgyfelId-vel: {munka.UgyfelId}");
            }

            _context.Munkak.Add(munka);
            await _context.SaveChangesAsync();
            munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);
            return munka;
        }

        public async Task<bool> UpdateMunkaAsync(int id, Munka munka)
        {
            if (id != munka.MunkaId)
            {
                return false; // or ArgumentException
            }

            var ugyfel = await _ugyfelService.GetUgyfelByIdAsync(munka.UgyfelId);
            if (ugyfel == null)
            {
                throw new ArgumentException($"Nem létezik ügyfél a megadott UgyfelId-vel: {munka.UgyfelId}");
            }

            var munkaJelenlegi = await _context.Munkak.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.MunkaId == id); //dont follow this EF instance

            if (munkaJelenlegi == null)
            {
                return false;
            }

            if(munkaJelenlegi.Allapot != munka.Allapot)
            {
                bool validTransition = false;

                switch (munkaJelenlegi.Allapot)
                {
                    case MunkaAllapot.Felvett:
                        if(munka.Allapot == MunkaAllapot.ElvegzesAlatt)
                        {
                            validTransition = true;
                        }
                        break;
                    case MunkaAllapot.ElvegzesAlatt:
                        if(munka.Allapot == MunkaAllapot.Befejezett)
                        {
                            validTransition = true;
                        }
                        break;
                    case MunkaAllapot.Befejezett:
                        if(munka.Allapot == MunkaAllapot.Befejezett)
                        {
                            validTransition = true;
                        }
                        break;
                }

                if (!validTransition)
                {
                    throw new InvalidOperationException($"Érvénytelen állapotváltás: '{munkaJelenlegi.Allapot}' állapotról nem lehet '{munka.Allapot}' állapotra váltani.");
                }
            }

            _context.Entry(munka).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MunkaExistsAsync(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteMunkaAsync(int id)
        {
            var munka = await _context.Munkak.FindAsync(id);
            if (munka == null)
            {
                return false;
            }
            _context.Munkak.Remove(munka);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> MunkaExistsAsync(int id)
        {
            return await _context.Munkak.AnyAsync(e => e.MunkaId == id);
        }
    }
}