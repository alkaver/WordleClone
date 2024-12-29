using System.ComponentModel.DataAnnotations;

namespace CalendarAPI.Models
{
    public class Guess
    {
        [Key]
        public int GuessId { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public string GuessWord { get; set; }
        public int AttemptNumber { get; set; }  // Która to próba (1-6)
        public bool GuessedCorrectly { get; set; }
        public DateTime GuessTime { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Game Game { get; set; }
    }

}
