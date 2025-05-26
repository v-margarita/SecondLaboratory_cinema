using System.ComponentModel.DataAnnotations;

namespace Cinema_laboratory2.ViewModels
{
    public class TicketEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Оберіть кінотеатр")]
        [Display(Name = "Кінотеатр")]
        public int CinemaId { get; set; }

        [Required(ErrorMessage = "Оберіть сеанс")]
        [Display(Name = "Сеанс")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Оберіть місце")]
        [Display(Name = "Місце")]
        public string SeatNumber { get; set; }

        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(1, 10000, ErrorMessage = "Ціна має бути від 1 до 10000")]
        [Display(Name = "Ціна")]
        public decimal Price { get; set; }

        public List<string> AvailableSeats { get; set; } = new();
    }
}