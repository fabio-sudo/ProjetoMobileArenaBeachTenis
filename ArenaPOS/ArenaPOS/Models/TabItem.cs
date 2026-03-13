using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ArenaPOS.Models
{
    [Table("TabItems")]
    public class TabItem
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        public int ApiId { get; set; }

        [ForeignKey(typeof(Tab))]
        public int TabLocalId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        
        public bool IsSynced { get; set; } = false;
    }
}
