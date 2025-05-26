using Cinema_laboratory2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema_laboratory2
{
    public class TicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) => _context = context;

        public List<Ticket> GetAll() => _context.Tickets.ToList();
        public Ticket? GetById(int id) => _context.Tickets.Find(id);
        public void Add(Ticket ticket) { _context.Tickets.Add(ticket); _context.SaveChanges(); }
        public void Update(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket != null) { _context.Tickets.Remove(ticket); _context.SaveChanges(); }
        }
    }

}
