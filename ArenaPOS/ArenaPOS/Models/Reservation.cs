using SQLite;

namespace ArenaPOS.Models
{
    [Table("Reservations")]
    public class Reservation
    {
        [PrimaryKey]
        public int ApiId { get; set; }

        public string CourtName { get; set; }

        public string Time { get; set; }

        public string PlayerName { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } // e.g. Pending, Paid
    }
}
