using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentals.Data.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerDay { get; set; }
        public List<string> ImageUrls { get; set; } // Store links to images
        public bool IsAvailable { get; set; }
        public int EnvironmentalRating { get; set; }
        public int NumberSeats { get; set; }
        public bool IsElectric {  get; set; }
        public bool IsAutomatic { get; set; }
        
    }
}