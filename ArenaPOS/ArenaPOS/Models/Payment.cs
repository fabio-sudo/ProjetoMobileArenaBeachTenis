using SQLite;
using System;

namespace ArenaPOS.Models
{
    [Table("Payments")]
    public class Payment
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        public int ApiId { get; set; }
        
        public string Source { get; set; } // Sale, Tab, Reservation, Adjustment

        public int SourceId { get; set; } 

        public decimal Amount { get; set; }

        public string Method { get; set; } // Cash, Card, Pix

        public DateTime Date { get; set; }

        public bool IsSynced { get; set; } = false;
    }
}
