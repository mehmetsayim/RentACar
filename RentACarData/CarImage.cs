using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace RentACarData
{
    public class CarImage :EntityBase
    {
        public Guid CarId { get; set; }

        public byte[] Image { get; set; }

        public virtual Car? Car { get; set; }
    }

    public class CarImageEntityTypeConfiguration : IEntityTypeConfiguration<CarImage>
    {
        public void Configure(EntityTypeBuilder<CarImage> builder)
        {
            builder
                .Property(p => p.Image)
                .IsRequired();
        }
    }
}
