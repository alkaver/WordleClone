using Microsoft.AspNetCore.Identity;

namespace CalendarAPI.Models
{
    public class User : IdentityUser
    {
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int GamesPlayedTotal { get; set; }
    }

}
