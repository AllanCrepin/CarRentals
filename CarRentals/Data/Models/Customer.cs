using System.ComponentModel.DataAnnotations;

namespace CarRentals.Data.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn krävs.")]
        [StringLength(50, ErrorMessage = "Namn och efternamn får inte vara större än 50 tecken.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mejladdress krävs.")]
        [EmailAddress(ErrorMessage = "Ogiltig mejladdress")]
        public string Email { get; set; }

        [StringLength(20, MinimumLength = 4, ErrorMessage = "Lösenordet måste vara mellan 4 och 20 tecken.")]
        [Required(ErrorMessage = "Lösenord krävs.")]
        public string Password { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
