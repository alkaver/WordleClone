using System.ComponentModel.DataAnnotations;

namespace CalendarAPI.Models
{
    public class Word
    {
        [Key]
        public int WordId { get; set; }
        public string WordOfTheDay { get; set; }
        public DateTime Date { get; set; }
    }

}
