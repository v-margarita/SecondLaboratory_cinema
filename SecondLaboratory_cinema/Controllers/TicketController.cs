using Cinema_laboratory2.Models;
using Cinema_laboratory2.Services;
using Cinema_laboratory2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_laboratory2.Controllers
{
    [Route("tickets")]
    public class TicketController : Controller
    {
        private readonly TicketService _service;
        private readonly AppDbContext _context;

        public TicketController(TicketService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var tickets = _context.Tickets
                .Include(t => t.Session)
                    .ThenInclude(s => s.Cinema)
                .Include(t => t.Session)
                    .ThenInclude(s => s.Movie)
                .ToList();

            return View(tickets);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View(new TicketCreateViewModel());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Tickets.AnyAsync(t =>
                    t.SessionId == model.SessionId &&
                    t.SeatNumber == model.SeatNumber))
                {
                    ModelState.AddModelError("SeatNumber", "Це місце вже зайняте");
                    LoadCreateViewData(model.CinemaId, model.SessionId);
                    return View(model);
                }

                var ticket = new Ticket
                {
                    Price = model.Price,
                    SeatNumber = model.SeatNumber,
                    SessionId = model.SessionId
                };

                await _service.CreateTicketAsync(ticket);
                return RedirectToAction("Index");
            }

            LoadCreateViewData(model.CinemaId, model.SessionId);
            return View(model);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _service.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var model = new TicketEditViewModel
            {
                Id = ticket.Id,
                Price = ticket.Price,
                SeatNumber = ticket.SeatNumber,
                SessionId = ticket.SessionId,
                CinemaId = ticket.Session.CinemaId
            };

            LoadEditViewData(model.CinemaId, model.SessionId, model.SeatNumber);
            return View(model);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var isSeatTaken = await _context.Tickets
                        .AnyAsync(t => t.SessionId == model.SessionId &&
                                     t.SeatNumber == model.SeatNumber &&
                                     t.Id != model.Id);

                    if (isSeatTaken)
                    {
                        ModelState.AddModelError("SeatNumber", "Це місце вже зайняте");
                        LoadEditViewData(model.CinemaId, model.SessionId, model.SeatNumber);
                        return View(model);
                    }

                    var ticket = new Ticket
                    {
                        Id = model.Id,
                        Price = model.Price,
                        SeatNumber = model.SeatNumber,
                        SessionId = model.SessionId
                    };

                    await _service.UpdateTicketAsync(ticket);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Помилка при оновленні: " + ex.Message);
                }
            }

            LoadEditViewData(model.CinemaId, model.SessionId, model.SeatNumber);
            return View(model);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.RemoveTicketAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("getsessions/{cinemaId}")]
        public async Task<IActionResult> GetSessions(int cinemaId)
        {
            var sessions = await _context.Sessions
                .Where(s => s.CinemaId == cinemaId)
                .Include(s => s.Movie)
                .Select(s => new
                {
                    id = s.Id,
                    movieTitle = s.Movie.Title,
                    startTime = s.StartTime.ToString("HH:mm")
                })
                .ToListAsync();

            return Json(sessions);
        }

        [HttpGet("getavailableseats/{sessionId}")]
        public async Task<IActionResult> GetAvailableSeats(int sessionId)
        {
            var takenSeats = await _context.Tickets
                .Where(t => t.SessionId == sessionId)
                .Select(t => t.SeatNumber)
                .ToListAsync();

            var allSeats = Enumerable.Range(1, 50).Select(n => $"A{n}").ToList();
            var availableSeats = allSeats.Except(takenSeats).ToList();

            return Json(availableSeats);
        }

        private void LoadCreateViewData(int? cinemaId = null, int? sessionId = null)
        {
            ViewBag.Cinemas = _context.Cinemas.ToList();

            if (cinemaId.HasValue)
            {
                ViewBag.Sessions = _context.Sessions
                    .Where(s => s.CinemaId == cinemaId)
                    .Include(s => s.Movie)
                    .Select(s => new SessionInfo
                    {
                        Id = s.Id,
                        FullInfo = $"{s.Movie.Title} - {s.StartTime:HH:mm}"
                    })
                    .ToList();
            }

            if (sessionId.HasValue)
            {
                var takenSeats = _context.Tickets
                    .Where(t => t.SessionId == sessionId)
                    .Select(t => t.SeatNumber)
                    .ToList();

                var allSeats = Enumerable.Range(1, 50).Select(n => $"A{n}").ToList();
                ViewBag.AvailableSeats = allSeats.Except(takenSeats).ToList();
            }
        }

        private void LoadEditViewData(int? cinemaId = null, int? sessionId = null, string currentSeat = null)
        {
            ViewBag.Cinemas = _context.Cinemas.ToList();

            if (cinemaId.HasValue)
            {
                ViewBag.Sessions = _context.Sessions
                    .Where(s => s.CinemaId == cinemaId)
                    .Include(s => s.Movie)
                    .Select(s => new SessionInfo
                    {
                        Id = s.Id,
                        FullInfo = $"{s.Movie.Title} - {s.StartTime:HH:mm}"
                    })
                    .ToList();
            }

            if (sessionId.HasValue)
            {
                var takenSeats = _context.Tickets
                    .Where(t => t.SessionId == sessionId && t.SeatNumber != currentSeat)
                    .Select(t => t.SeatNumber)
                    .ToList();

                var allSeats = Enumerable.Range(1, 50).Select(n => $"A{n}").ToList();
                ViewBag.AvailableSeats = allSeats.Except(takenSeats).ToList();

                if (currentSeat != null && !ViewBag.AvailableSeats.Contains(currentSeat))
                {
                    ViewBag.AvailableSeats.Add(currentSeat);
                }
            }
        }
    }

    public class SessionInfo
    {
        public int Id { get; set; }
        public string FullInfo { get; set; }
    }
}