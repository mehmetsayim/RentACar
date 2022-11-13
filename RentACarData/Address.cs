using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace RentACarData
{
    public class Address :EntityBase
    {
        public Guid ApplicationUserId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

    public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder
                .HasMany(p => p.Orders)
                .WithOne(p => p.DeliveryAddress)
                .HasForeignKey(p => p.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
