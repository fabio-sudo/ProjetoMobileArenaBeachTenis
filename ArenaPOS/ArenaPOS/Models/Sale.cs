using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace ArenaPOS.Models
{
    [Table("Sales")]
    public class Sale
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; } // Internal SQLite ID for offline Sync

        public int ApiId { get; set; } // ID on the backend (0 if not synced)

        public decimal Total { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public bool IsSynced { get; set; } = false;

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<SaleItem> Items { get; set; } = new();
    }
}