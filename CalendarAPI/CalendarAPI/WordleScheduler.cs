namespace CalendarAPI
{
    using CalendarAPI.Data;
    using CalendarAPI.Models;
    using Microsoft.EntityFrameworkCore;

    public class WordleScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WordleScheduler> _logger;
        private readonly string[] _words = { "apple", "grape", "melon", "peach", "berry" }; // Lista słów do losowania

        public WordleScheduler(IServiceScopeFactory scopeFactory, ILogger<WordleScheduler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndGenerateWord();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in WordleScheduler");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Sprawdzaj co minutę
            }
        }

        private async Task CheckAndGenerateWord()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.UtcNow.Date;

            // Sprawdź, czy gra na dzisiaj już istnieje
            var existingGame = await dbContext.Games
                .FirstOrDefaultAsync(g => g.GameDate == now);

            if (existingGame == null)
            {
                var random = new Random();
                var word = _words[random.Next(_words.Length)];

                var newGame = new Game
                {
                    WordOfTheDay = word,
                    GameDate = now
                };

                dbContext.Games.Add(newGame);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Generated new word for {now}: {word}");
            }
        }
    }

}
