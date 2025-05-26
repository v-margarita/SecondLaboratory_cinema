using Cinema_laboratory2.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema_laboratory2.Services
{
    public class TicketService
    {
        private readonly TicketRepository _repo;
        private readonly AppDbContext _context;
        public TicketService(TicketRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }


        public List<Ticket> GetAllTickets() => _repo.GetAll();
        public Ticket? GetTicketById(int id) => _repo.GetById(id);
        public void CreateTicket(Ticket ticket) => _repo.Add(ticket);
        public void EditTicket(Ticket ticket) => _repo.Update(ticket);
        public void RemoveTicket(int id) => _repo.Delete(id);

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Session)
                .ThenInclude(s => s.Cinema)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            await Task.Run(() => _repo.Add(ticket));
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await Task.Run(() => _repo.Update(ticket));
        }

        public async Task RemoveTicketAsync(int id)
        {
            await Task.Run(() => _repo.Delete(id));
        }
    }
}
