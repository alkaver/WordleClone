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
            await ResetUserStreaks();
        }
    }

    private async Task ResetUserStreaks()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var today = DateTime.UtcNow.Date;

        var users = dbContext.Users
            .Include(u => u.GameRecords)
            .ToList();

        foreach (var user in users)
        {
            // Sprawdź, czy gracz grał dzisiaj
            var playedToday = user.GameRecords
                .Any(g => g.PlayedDate.Date == today);

            if (!playedToday)
            {
                user.CurrentStreak = 0;
                await dbContext.SaveChangesAsync();
            }
        }
    }

}
