using CalendarAPI.Data;
using CalendarAPI.Dtos;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalendarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public GameController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("play")]
        public async Task<IActionResult> PlayGame([FromBody] GameResultDto result)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Dodanie nowego rekordu gry
            var gameRecord = new GameRecord
            {
                UserId = user.Id,
                WonGame = result.WonGame,
                PlayedDate = DateTime.UtcNow
            };

            _context.GameRecords.Add(gameRecord);
            await _context.SaveChangesAsync();
            Console.WriteLine(result.WonGame);

            // Aktualizacja streaków
            user.GamesPlayedTotal += 1;
            if (result.WonGame)
            {
                user.CurrentStreak += 1;
                if (user.CurrentStreak > user.MaxStreak)
                {
                    user.MaxStreak = user.CurrentStreak;
                }
            }
            else
            {
                user.CurrentStreak = 0;
            }

            await _userManager.UpdateAsync(user);
            return Ok("Game result recorded.");
        }
    }

}
