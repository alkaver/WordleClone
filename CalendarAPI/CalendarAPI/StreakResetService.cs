using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class StreakResetService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<StreakResetService> _logger;

    public StreakResetService(IServiceScopeFactory scopeFactory, ILogger<StreakResetService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            // Czekamy do północy
            var nextRun = now.Date.AddDays(1).AddHours(0);
            var delay = nextRun - now;

            _logger.LogInformation($"Waiting until {nextRun} to reset streaks...");

            await Task.Delay(delay, stoppingToken);

            // Reset streaks po północy
            await ResetUserStreaks(stoppingToken);
        }
    }

    private async Task ResetUserStreaks(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            // Sprawdzenie, czy można połączyć się z bazą danych
            if (!await dbContext.Database.CanConnectAsync(stoppingToken))
            {
                _logger.LogWarning("Database is not available. Skipping streak reset.");
                return;
            }

            var today = DateTime.UtcNow.Date;

            // Pobierz użytkowników i ich rekordy gry
            var users = await dbContext.Users
                .Include(u => u.GameRecords)
                .AsNoTracking() // Optymalizacja - nie śledzimy zmian
                .ToListAsync(stoppingToken);

            foreach (var user in users)
            {
                // Sprawdź, czy użytkownik grał dzisiaj
                var playedToday = user.GameRecords
                    .Any(g => g.PlayedDate.Date == today);

                if (!playedToday)
                {
                    user.CurrentStreak = 0;
                    _logger.LogInformation($"User {user.Id} streak reset to 0.");
                }
            }

            // Zapisz zmiany w bazie
            dbContext.Users.UpdateRange(users);
            await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting user streaks.");
        }
    }

}
