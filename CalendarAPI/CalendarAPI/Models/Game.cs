using System.ComponentModel.DataAnnotations;

namespace CalendarAPI.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        public string WordOfTheDay { get; set; }
        public DateTime GameDate { get; set; }
    }

}
