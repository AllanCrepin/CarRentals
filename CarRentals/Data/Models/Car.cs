using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentals.Data.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Modell krävs.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "År krävs")]
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "Pris per dag krävs.")]
        public decimal PricePerDay { get; set; }
        public List<string> ImageUrls { get; set; } // Store links to images

        [Required]
        public bool IsAvailable { get; set; }
        [Required(ErrorMessage = "Miljöbetyg krävs.")]
        public int EnvironmentalRating { get; set; }
        [Required(ErrorMessage = "Antal säten krävs.")]
        public int NumberSeats { get; set; }
        [Required(ErrorMessage = "Bilens typ krävs.")]
        public bool IsElectric {  get; set; }

        [Required(ErrorMessage = "Bilens växeltyp krävs.")]
        public bool IsAutomatic { get; set; }
        
    }
}