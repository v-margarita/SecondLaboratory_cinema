namespace Cinema_laboratory2.Models
{
  
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Session> Sessions { get; set; } = new();
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DurationMinutes { get; set; }
        public List<Session> Sessions { get; set; } = new();
    }

    public class Session
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public List<Ticket> Tickets { get; set; } = new();
    }

    public class Ticket
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string SeatNumber { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}
