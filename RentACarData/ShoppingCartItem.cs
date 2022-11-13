using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace RentACarData
{
    public class ShoppingCartItem : EntityBase
    {
        public Guid CarId { get; set; }

        public Guid ApplicationUserId { get; set; }

        public int Quantity { get; set; }

        public virtual Car? Car { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }

    public class ShoppingCartEntityTypeConfiguration : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {

        }
    }
}
