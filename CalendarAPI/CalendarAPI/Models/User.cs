using Microsoft.AspNetCore.Identity;

namespace CalendarAPI.Models
{
    public class User : IdentityUser
    {
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int GamesPlayedTotal { get; set; }

        // Relacja jeden-do-wielu
        public ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();
    }


}
