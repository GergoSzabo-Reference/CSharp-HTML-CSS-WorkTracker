using Autoszerelo_API.Data;
using Autoszerelo_Shared;
using Autoszerelo_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autoszerelo_API.Controllers
{
    [Route("api/[controller]")] // /api/Munkak
    [ApiController]
    public class MunkakController : ControllerBase
    {
        private readonly AutoszereloDbContext _context;
        private readonly WorkHourEstimationService _estimationService;

        public MunkakController(AutoszereloDbContext context, WorkHourEstimationService estimationService)
        {
            _context = context;
            _estimationService = estimationService;
        }

        // GET: api/Munkak
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Munka>>> GetMunkak()
        {
            var munkak = await _context.Munkak.ToListAsync();

            foreach(var munka in munkak)
            {
                munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);
            }

            return Ok(munkak);
        }

        // GET: api/Munkak/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Munka>> GetMunka(int id)
        {
            /*Munka contains UgyfelId
                -> .Include(m => m.Ugyfel)
            */
            var munka = await _context.Munkak.FindAsync(id);

            if (munka == null)
            {
                return NotFound();
            }

            munka.BecsultMunkaorak = _estimationService.CalculateEstimatedHours(munka);

            return Ok(munka);
        }

        // POST: api/Munkak
        [HttpPost]
        public async Task<ActionResult<Munka>> PostMunka([FromBody] Munka munka)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400
            }

            var ugyfelLetezik = await _context.Ugyfelek.AnyAsync(u => u.UgyfelId == munka.UgyfelId);
            if (!ugyfelLetezik)
            {
                return BadRequest(new { message = $"Nem létezik ügyfél a megadott UgyfelId-vel: {munka.UgyfelId}" });
            }

            munka.MunkaId = 0; // to handle as new one

            _context.Munkak.Add(munka);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMunka), new { id = munka.MunkaId }, munka); // 201
        }

        // PUT: api/Munkak/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Munka>> PutMunka(int id, [FromBody] Munka munka)
        {
            if(id != munka.MunkaId)
            {
                return BadRequest("Az útvonalban létező ID != a body-ban megadott munkaID-vel.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ugyfelLetezik = await _context.Ugyfelek.AnyAsync(u => u.UgyfelId == munka.UgyfelId);
            if (!ugyfelLetezik)
            {
                return BadRequest($"Nem létezik ügyfél a megadott ugyfelId-val: {munka.UgyfelId}");
            }

            _context.Entry(munka).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MunkaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); //204
        }

        // DELETE: api/Munkak/{id}
        [HttpDelete("id")]
        public async Task<ActionResult<Munka>> DeleteMunka(int id)
        {
            var munka = await _context.Munkak.FindAsync(id);

            if (munka == null)
            {
                return NotFound();
            }

            _context.Munkak.Remove(munka);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MunkaExists(int id)
        {
            return _context.Munkak.Any(m => m.MunkaId == id);
        }
    }
}