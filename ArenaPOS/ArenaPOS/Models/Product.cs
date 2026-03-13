using SQLite;

namespace ArenaPOS.Models
{
    [Table("Products")]
    public class Product
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public bool IsActive { get; set; }
    }
}
