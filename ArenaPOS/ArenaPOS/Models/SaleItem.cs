using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ArenaPOS.Models
{
    [Table("SaleItems")]
    public class SaleItem
    {
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        [ForeignKey(typeof(Sale))]
        public int SaleLocalId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}