//using Autoszerelo_API.Data;
using Autoszerelo_API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autoszerelo.Shared;
using Autoszerelo_Shared;
using Autoszerelo_API.Services;

namespace Autoszerelo_API.Controllers
{
    [Route("api/[controller]")] // /api/ugyfelek
    [ApiController] // model validation errors, paramater binding source, errors msgs
    public class UgyfelekController : ControllerBase
    {
        private readonly IUgyfelService _ugyfelService;

        public UgyfelekController(IUgyfelService ugyfelService) //DI
        {
            _ugyfelService = ugyfelService;
        }

        //GET: api/Ugyfelek
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ugyfel>>> GetUgyfelek() 
        //ActionResult: return with HTTP codes
        {
            var ugyfelek = await _ugyfelService.GetUgyfelekAsync();
            return Ok(ugyfelek);
        }

        //GET: api/Ugyfelek/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ugyfel>> GetUgyfel(int id)
        {
            var ugyfel = await _ugyfelService.GetUgyfelByIdAsync(id);

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
            var ugyfel = await _ugyfelService.GetUgyfelByIdAsync(ugyfelId);
            if (ugyfel == null)
            {
                return NotFound($"Nem található ügyfél a megadott ID-vel: {ugyfelId}");
            }

            var munkak = await _ugyfelService.GetMunkakByUgyfelIdAsync(ugyfelId);

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

            var createdUgyfel = await _ugyfelService.CreateUgyfelAsync(ugyfel);

            return CreatedAtAction(nameof(GetUgyfel), new { id = createdUgyfel.UgyfelId }, createdUgyfel); //201, created
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

            var success = await _ugyfelService.UpdateUgyfelAsync(id, ugyfel);
            if (!success)
            {
                return NotFound();
            }

            return NoContent(); //204
        }

        //DELETE: api/Ugyfelek/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Ugyfel>> DeleteUgyfel(int id)
        {
            var success = await _ugyfelService.DeleteUgyfelAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent(); // 204
        }

        /*private bool UgyfelExists(int id)
        {
            return _ugyfelService.Ugyfelek.Any(e => e.UgyfelId == id);
        }*/
    }
}
