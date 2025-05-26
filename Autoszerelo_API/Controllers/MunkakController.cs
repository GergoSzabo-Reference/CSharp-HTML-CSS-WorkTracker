using Autoszerelo_API.Data;
using Autoszerelo_Shared;
using Autoszerelo_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autoszerelo_API.Interfaces;

namespace Autoszerelo_API.Controllers
{
    [Route("api/[controller]")] // /api/Munkak
    [ApiController]
    public class MunkakController : ControllerBase
    {
        private readonly AutoszereloDbContext _context = default!; //value will be 100% assigned = no warning
        private readonly WorkHourEstimationService _estimationService = default!;
        private readonly IMunkaService _munkaService;

        public MunkakController(IMunkaService munkaService)
        {
            _munkaService = munkaService;
        }

        // GET: api/Munkak
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Munka>>> GetMunkak()
        {
            var munkak = await _munkaService.GetMunkakAsync();

            return Ok(munkak);
        }

        // GET: api/Munkak/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Munka>> GetMunka(int id)
        {
            /*Munka contains UgyfelId
                -> .Include(m => m.Ugyfel)
            */

            var munka = await _munkaService.GetMunkaByIdAsync(id);
            if(munka == null)
            {
                return NotFound();
            }

            return Ok(munka);
        }

        // POST: api/Munkak
        [HttpPost]
        public async Task<ActionResult<Munka>> PostMunka([FromBody] Munka munka)
        {
            if (!ModelState.IsValid) //after conversion from json, validations will be checked
            {
                return BadRequest(ModelState); // 400
            }

            try
            {
                var createdMunka = await _munkaService.CreateMunkaAsync(munka);
                return CreatedAtAction(nameof(GetMunka), new { id = createdMunka.MunkaId }, createdMunka); // 201
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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

            try
            {
                var success = await _munkaService.UpdateMunkaAsync(id, munka);
                if (!success)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Munkak/{id}
        [HttpDelete("id")]
        public async Task<ActionResult<Munka>> DeleteMunka(int id)
        {
            var success = await _munkaService.DeleteMunkaAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        private bool MunkaExists(int id)
        {
            return _context.Munkak.Any(m => m.MunkaId == id);
        }
    }
}