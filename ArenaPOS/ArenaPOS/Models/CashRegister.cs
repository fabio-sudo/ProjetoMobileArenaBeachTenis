using SQLite;
using System;

namespace ArenaPOS.Models
{
    [Table("CashRegisters")]
    public class CashRegister
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        public int ApiId { get; set; }

        public DateTime DateOpened { get; set; }

        public DateTime? DateClosed { get; set; }

        public decimal StartingBalance { get; set; }

        public decimal ClosingBalance { get; set; }

        public string Status { get; set; } // Open, Closed

        public bool IsSynced { get; set; } = false;
    }
}
