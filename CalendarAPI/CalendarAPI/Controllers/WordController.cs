using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // Jeśli słowo nie zostało znalezione, wywołaj metodę do aktualizacji
            if (todayWord == null)
            {
                await UpdateWordOfTheDayAsync();
                // Po aktualizacji, pobierz nowe słowo
                todayWord = await _context.Words
                    .Where(w => w.Date.Date == DateTime.UtcNow.Date)
                    .FirstOrDefaultAsync();
            }

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

        // Metoda odpowiedzialna za dodanie nowego słowa dnia
        private async Task UpdateWordOfTheDayAsync()
        {
            // Lista dostępnych słów
            var wordsList = new List<string>
            {
                "apple", "bbbbb", "ccccc", "ddddd", "eeeee", "fffff", "ggggg", "hhhhh", "iiiii", "jjjjj"
            };

            // Losowanie nowego słowa
            var randomIndex = new Random().Next(wordsList.Count);
            var wordOfTheDay = wordsList[randomIndex];

            // Sprawdzanie, czy słowo dnia już istnieje
            var newWord = new Word
            {
                WordOfTheDay = wordOfTheDay,
                Date = DateTime.UtcNow
            };

            // Dodanie słowa do bazy danych
            _context.Words.Add(newWord);
            await _context.SaveChangesAsync();
        }
    }
}

