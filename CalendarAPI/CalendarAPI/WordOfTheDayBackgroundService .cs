using CalendarAPI.Data;
using CalendarAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarAPI
{
    public class WordOfTheDayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // Użycie IServiceProvider
        private readonly ILogger<WordOfTheDayBackgroundService> _logger;

        public WordOfTheDayBackgroundService(IServiceProvider serviceProvider, ILogger<WordOfTheDayBackgroundService> logger)
        {
            _serviceProvider = serviceProvider; // Uzyskiwanie AppDbContext z zakresu
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Czekamy do pierwszego uruchomienia
            await UpdateWordOfTheDayAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.UtcNow;
                var midnightTime = DateTime.UtcNow.Date.AddDays(1);
                var timeUntilMidnight = midnightTime - currentTime;

                _logger.LogInformation("Waiting until next midnight to update Word of the Day...");

                // Czekaj do północy
                await Task.Delay(timeUntilMidnight, stoppingToken);

                // Zaktualizuj słowo dnia po północy
                await UpdateWordOfTheDayAsync(stoppingToken);
            }
        }

        private async Task UpdateWordOfTheDayAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope()) // Tworzenie zakresu
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    // Sprawdzenie, czy baza danych jest dostępna
                    if (!await context.Database.CanConnectAsync(stoppingToken))
                    {
                        _logger.LogWarning("Database is not available. Skipping Word of the Day update.");
                        return; // Zakończ metodę, jeśli nie ma połączenia z bazą
                    }

                    var wordsList = new List<string>
                    {
                        "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew", "kiwi", "lemon"
                    };

                    var randomIndex = new Random().Next(wordsList.Count);
                    var wordOfTheDay = wordsList[randomIndex];

                    // Sprawdzenie, czy słowo dnia już istnieje w bazie dla dzisiejszego dnia
                    var existingWord = await context.Words
                        .Where(w => w.Date.Date == DateTime.UtcNow.Date)
                        .FirstOrDefaultAsync(stoppingToken);

                    if (existingWord == null)
                    {
                        var newWord = new Word
                        {
                            WordOfTheDay = wordOfTheDay,
                            Date = DateTime.UtcNow
                        };

                        context.Words.Add(newWord);
                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"Word of the Day has been set to: {wordOfTheDay}");
                    }
                    else
                    {
                        _logger.LogInformation($"Word of the Day already set for today: {existingWord.WordOfTheDay}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the Word of the Day.");
                }
            }
        }
    }
}
