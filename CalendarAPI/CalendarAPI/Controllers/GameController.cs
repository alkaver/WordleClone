using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public async Task<ActionResult<Game>> StartNewGame(string word)
        {
            var game = new Game
            {
                WordOfTheDay = word,
                GameDate = DateTime.UtcNow.Date
            };
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return Ok(game);
        }

        [HttpPost("guess")]
        public async Task<ActionResult> MakeGuess(int userId, int gameId, string guess)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null) return NotFound("Game not found");

            // Sprawdź, ile prób użytkownik już wykonał
            var userGuesses = await _context.Guesses
                .Where(g => g.UserId == userId && g.GameId == gameId)
                .OrderBy(g => g.AttemptNumber)
                .ToListAsync();

            if (userGuesses.Count >= 6)
            {
                return BadRequest("You have already used all 6 attempts for this game.");
            }

            bool correct = game.WordOfTheDay.Equals(guess, StringComparison.OrdinalIgnoreCase);

            var guessEntry = new Guess
            {
                UserId = userId,
                GameId = gameId,
                GuessWord = guess,
                AttemptNumber = userGuesses.Count + 1,
                GuessedCorrectly = correct
            };

            _context.Guesses.Add(guessEntry);
            await _context.SaveChangesAsync();

            var stats = await _context.Statistics
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (stats == null)
            {
                stats = new Statistic { UserId = userId };
                _context.Statistics.Add(stats);
            }

            // Jeżeli odgadł - aktualizacja streaku
            if (correct)
            {
                stats.GamesWon += 1;
                stats.CurrentStreak += 1;
                stats.LongestStreak = Math.Max(stats.LongestStreak, stats.CurrentStreak);
            }
            else if (userGuesses.Count == 5)  // Jeżeli to była 6 próba i nie odgadł
            {
                stats.CurrentStreak = 0;  // Reset streaku
            }

            stats.TotalGamesPlayed += 1;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Guess = guessEntry,
                Streak = stats.CurrentStreak,
                RemainingAttempts = 6 - (userGuesses.Count + 1)
            });
        }
        [HttpGet("game/{gameId}/user/{userId}")]
        public async Task<ActionResult<IEnumerable<Guess>>> GetUserGuesses(int userId, int gameId)
        {
            var guesses = await _context.Guesses
                .Where(g => g.UserId == userId && g.GameId == gameId)
                .OrderBy(g => g.AttemptNumber)
                .ToListAsync();

            if (guesses == null || guesses.Count == 0)
            {
                return NotFound("No guesses found for this game.");
            }

            return Ok(guesses);
        }
    }
}
