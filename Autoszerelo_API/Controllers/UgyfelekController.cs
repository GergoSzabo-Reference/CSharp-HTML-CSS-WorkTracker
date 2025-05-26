using Autoszerelo_API.Data;
using Autoszerelo_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autoszerelo.Shared;
using Autoszerelo_Shared;

namespace Autoszerelo_API.Controllers
{
    [Route("api/[controller]")] // /api/ugyfelek
    [ApiController] // model validation errors, paramater binding source, errors msgs
    public class UgyfelekController : ControllerBase
    {
        private readonly AutoszereloDbContext _context;
        private readonly WorkHourEstimationService _estimation_service;

        public UgyfelekController(AutoszereloDbContext context, WorkHourEstimationService estimation_service) //DI
        {
            _context = context;
            _estimation_service = estimation_service;
        }

        //GET: api/Ugyfelek
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ugyfel>>> GetUgyfelek() 
        //ActionResult: return with HTTP codes
        {
            var ugyfelek = await _context.Ugyfelek.ToListAsync(); //Async load table "Ugyfelek"
            return Ok(ugyfelek);
        }

        //GET: api/Ugyfelek/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ugyfel>> GetUgyfel(int id)
        {
            var ugyfel = await _context.Ugyfelek.FindAsync(id);

            if(ugyfel== null)
            {
                return NotFound();
            }

            return Ok(ugyfel);
        }

        //GET: api/Ugyfelek/{ugyfelId}/Munkak
        [HttpGet("{ugyfelId}/munkak")]
        public async Task<ActionResult<IEnumerable<Munka>>> GetMunkakUgyfelhez(int ugyfelId)
        {
            if (!UgyfelExists(ugyfelId))
            {
                return NotFound($"Nem található ügyfél a megadott ID-vel: {ugyfelId}");
            }

            var munkak = await _context.Munkak
                .Where(m => m.UgyfelId == ugyfelId)
                .ToListAsync();

            if(munkak == null)
            {
                return NotFound("Nem találhatóak munkák az adott ügyfélhez.");
            }

            if(_estimation_service == null)
            {
                throw new InvalidOperationException("Nincs injektálva a WorkHoursEstimationService.");
            }

            foreach(var munka in munkak)
            {
                munka.BecsultMunkaorak = _estimation_service.CalculateEstimatedHours(munka);
            }

            return Ok(munkak);
        }

        //POST: api/Ugyfelek
        [HttpPost]
        public async Task<ActionResult<Ugyfel>> PostUgyfel([FromBody] Ugyfel ugyfel)
        {
            if (!ModelState.IsValid) // is the data validation satisfied?
            {
                return BadRequest(ModelState); // 400
            }

            ugyfel.UgyfelId = 0;

            _context.Ugyfelek.Add(ugyfel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUgyfel), new { id = ugyfel.UgyfelId }, ugyfel); //201, created
            //location header: new item's url
        }

        //PUT: api/Ugyfelek/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Ugyfel>> PutUgyfel(int id, [FromBody] Ugyfel ugyfel)
        {
            if(id != ugyfel.UgyfelId)
            {
                return BadRequest("Az útvonalban szereplő ID nem egyezik a kérés törzsében (body) lévő ügyfél ID-jével.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(ugyfel).State = EntityState.Modified; //update sql | client already exists

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) // someone else has already modified the given record
            {
                if (!UgyfelExists(id)){
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); //204
        }

        //DELETE: api/Ugyfelek/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ugyfel>> DeleteUgyfel(int id)
        {
            var ugyfel = await _context.Ugyfelek.FindAsync(id);

            if(ugyfel == null)
            {
                return NotFound();
            }

            _context.Ugyfelek.Remove(ugyfel);

            await _context.SaveChangesAsync();

            return NoContent(); // 204
        }

        private bool UgyfelExists(int id)
        {
            return _context.Ugyfelek.Any(e => e.UgyfelId == id);
        }
    }
}
