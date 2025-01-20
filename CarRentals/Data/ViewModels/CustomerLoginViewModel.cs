using System.ComponentModel.DataAnnotations;

namespace CarRentals.Data.ViewModels
{
    public class CustomerLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
