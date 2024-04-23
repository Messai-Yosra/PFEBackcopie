using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stage_api.configuration;
using stage_api.Models;

namespace ProjetApiExcel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly dbContext _context;

        public PersonsController(dbContext context)
        {
            _context = context;
        }

        // GET: api/Persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Persons>>> GetPersons()
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            return await _context.Persons.Take(100)
                .ToListAsync();
        }

        // GET: api/Persons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Persons>> GetPersons(int id)
        {
          if (_context.Persons == null)
          {
              return NotFound();
          }
            var persons = await _context.Persons.FindAsync(id);

            if (persons == null)
            {
                return NotFound();
            }

            return persons;
        }

        // PUT: api/Persons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersons(int id, Persons persons)
        {
            if (id != persons.id)
            {
                return BadRequest();
            }

            _context.Entry(persons).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("addManyPersons")]
        public async Task<ActionResult<Persons>> PostPersons(List<Persons> persons)
        {
            if (_context.Persons == null)
            {
                return Problem("Entity set 'ProjetApiExcelContext.Persons'  is null.");
            }
            foreach (var person in persons)
            {
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();
            }
            

            return Ok();
        }

        // POST: api/Persons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Persons>> PostPersons(Persons persons)
        {
          if (_context.Persons == null)
          {
              return Problem("Entity set 'ProjetApiExcelContext.Persons'  is null.");
          }
            _context.Persons.Add(persons);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersons", new { id = persons.id }, persons);
        }

        // DELETE: api/Persons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersons(int id)
        {
            if (_context.Persons == null)
            {
                return NotFound();
            }
            var persons = await _context.Persons.FindAsync(id);
            if (persons == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(persons);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonsExists(int id)
        {
            return (_context.Persons?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
