namespace CalendarAPI.Dtos
{
    public class UserDetailDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string[]? Roles { get; set; }
        public int AccessFailedCount { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int GamesPlayedTotal { get; set; }
    }
}
