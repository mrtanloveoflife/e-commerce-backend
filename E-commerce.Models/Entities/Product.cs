using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] Image { get; set; }

        public virtual Category Category { get; set; }

        // Not implemented to simplify the example
        //public virtual ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        //public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}
