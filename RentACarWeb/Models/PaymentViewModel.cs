using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarWeb.Models
{
    public class PaymentViewModel
    {
        [Display(Name = "Kart No")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [RegularExpression("^[0-9]{15,16}$", ErrorMessage = "Lütfen geçerli bir {0} yazınız!")]
        public string CardNumber { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string CardHolderName { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        [Display(Name = "Son Kullanma T.")]
        public string Expiration => $"{Month}{Year}";

        [Display(Name = "CV2")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [RegularExpression("^[0-9]{3}$", ErrorMessage = "Lütfen geçerli bir {0} yazınız!")]
        public string CV2 { get; set; }
    }
}
