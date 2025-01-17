using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentals.Data.ViewModels
{
    public class BookingViewModel
    {
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }
        public bool IsCancelled { get; set; }
    }
}
