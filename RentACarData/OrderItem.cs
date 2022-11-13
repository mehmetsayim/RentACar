using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACarData
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid CarId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int DiscountRate { get; set; }

        [NotMapped]
        public decimal LineTotal => Quantity * Price;

        public virtual Order? Order { get; set; }
        public virtual Car? Car { get; set; }
        
    }

    public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

        }
    }
}
