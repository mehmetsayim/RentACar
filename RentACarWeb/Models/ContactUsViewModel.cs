using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarWeb.Models
{
    public class ContactUsViewModel
    {
        [Display(Name = "Ad/Soyad")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }

        [Display(Name = "E-Posta")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız!")]
        public string EMail { get; set; }

        [Display(Name = "İrtibat No.")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Mesaj")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Message { get; set; }
    }
}
