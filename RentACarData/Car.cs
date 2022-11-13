using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACarData
{
    public class Car : EntityBase
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Display(Name = "Açıklamalar")]
        [DataType(DataType.MultilineText)]
        public string? Descriptions { get; set; }

        public decimal Price { get; set; }

        public float Weight { get; set; }

        public decimal? DiscountedPrice { get; set; }

        [NotMapped]
        [Display(Name = "Liste Fiyatı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [DataType(DataType.Currency)]
        [RegularExpression("^[0-9]+(,[0-9]+)?$", ErrorMessage = "Lütfen geçerli bir fiyat yazınız!")]
        public string PriceText { get; set; }

        [NotMapped]
        [Display(Name = "İndirimli Fiyat")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [DataType(DataType.Currency)]
        [RegularExpression("^[0-9]+(,[0-9]+)?$", ErrorMessage = "Lütfen geçerli bir fiyat yazınız!")]
        public string DiscountedPriceText { get; set; }

        [NotMapped]
        public IEnumerable<IFormFile> ImageFiles { get; set; }

        [NotMapped]
        [Display(Name = "Kategori(ler)")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public IEnumerable<Guid> CategoryIds { get; set; }

        [NotMapped]
        public int DiscountRate => Price > 0 ? (int)Math.Round((Price - (DiscountedPrice ?? Price)) * 100 / Price) : 0;

        [NotMapped]
        public string ImageSrc => CarImages.Any() ? $"data:image/jpeg;base64, {Convert.ToBase64String(CarImages.First().Image)}" : "/images/no-image.png";

        [NotMapped]
        public IEnumerable<Guid> ImagesToDeleted { get; set; } = new List<Guid>();





        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        public virtual ICollection<CarImage> CarImages { get; set; } = new HashSet<CarImage>();

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new HashSet<ShoppingCartItem>();

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

    }

    public class CarEntityTypeConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder
                .Property(p => p.Name)
                .HasMaxLength(50);

            builder
                .HasMany(p => p.CarImages)
                .WithOne(p => p.Car)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.Comments)
                .WithOne(p => p.Car)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.ShoppingCartItems)
                .WithOne(p => p.Car)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Car)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property(p => p.Price)
                .HasPrecision(18, 4);

        }
    }
}
