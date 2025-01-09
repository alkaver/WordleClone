using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarAPI;
namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WordController(AppDbContext context)
        {
            _context = context;
        }

        // Metoda, która zwraca słowo dnia
        [HttpGet("wordoftheday")]
        public async Task<IActionResult> GetWordOfTheDay()
        {
            // Pobierz słowo dnia dla dzisiejszego dnia
            var todayWord = await _context.Words
                .Where(w => w.Date.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync();

            // Jeśli słowo zostało znalezione, zwróć je
            return Ok(todayWord.WordOfTheDay);
        }

        // (Opcjonalnie) Metoda, która ręcznie dodaje nowe słowo dnia (np. w celach administracyjnych)
        [HttpPost("setwordoftheday")]
        public async Task<IActionResult> SetWordOfTheDay([FromBody] string wordOfTheDay)
        {
            // Sprawdź, czy już istnieje słowo na dzisiaj
            var todayWord = await _context.Words
                .Where(w => w.Date.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync();

            if (todayWord != null)
            {
                return Conflict("Word for today has already been set.");
            }

            // Dodaj nowe słowo dnia do bazy
            var newWord = new Word
            {
                WordOfTheDay = wordOfTheDay,
                Date = DateTime.UtcNow
            };

            _context.Words.Add(newWord);
            await _context.SaveChangesAsync();

            return Ok($"Word of the day set to: {wordOfTheDay}");
        }
    }
}

