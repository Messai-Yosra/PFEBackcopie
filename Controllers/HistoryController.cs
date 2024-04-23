using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stage_api.DTO;
using stage_api.Models;

namespace stage_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly dbContext _context;

        public HistoryController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversionHistory>>> GetHistory()
        {
            if (_context.Persons == null)
            {
                return NotFound();
            }
            return await _context.ConversionHistory.ToListAsync();
        }

        [HttpGet]
        [Route("historyByFile")]
        public  ActionResult<ConversionHistory> GetHistory(string fileName)
        {
            if (_context.Persons == null)
            {
                return NotFound();
            }
            return Ok(_context.ConversionHistory.Where(f=>f.FileName==fileName)
                .FirstOrDefault());
        }

        // POST: api/history
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ConversionHistory>> PostHistory(ConversionHistory history)
        {
            if (_context.ConversionHistory == null)
            {
                return Problem("Entity set 'ProjetApiExcelContext.Persons'  is null.");
            }
            _context.ConversionHistory.Add(history);
            await _context.SaveChangesAsync();

            return CreatedAtAction("getHistory", new { id = history.Id }, history);
        }

        [HttpGet]
        [Route("stats")]
        public async Task<Object> getstats()
        {
            var services = _context.ConversionHistory.Select(h => new Stats
            {
                name = h.FileName,
                value = _context.ConversionHistory.Where(f=>f.FileName==h.FileName).Count()
            })
        .ToList();

            return Ok(services);
        }
    }
}
