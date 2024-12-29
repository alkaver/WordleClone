using System.ComponentModel.DataAnnotations;

namespace CalendarAPI.Models
{
    public class Statistic
    {
        [Key]
        public int StatId { get; set; }
        public int UserId { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int GamesWon { get; set; }

        public User User { get; set; }
    }

}
