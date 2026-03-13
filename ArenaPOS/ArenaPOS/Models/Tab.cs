using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace ArenaPOS.Models
{
    [Table("Tabs")]
    public class Tab
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        public int ApiId { get; set; }

        public string CustomerName { get; set; }

        public string Status { get; set; } // Open, Closed

        public decimal TotalAmount { get; set; }

        public bool IsSynced { get; set; } = false;

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TabItem> Items { get; set; } = new();
    }
}
