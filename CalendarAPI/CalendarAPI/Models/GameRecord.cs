using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CalendarAPI.Models
{
    public class GameRecord
    {
        [Key]
        public int GameId { get; set; }
        public bool WonGame { get; set; }
        public DateTime PlayedDate { get; set; }

        // Klucz obcy
        public string UserId { get; set; }
        public User User { get; set; }
    }

}
