using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RentACarWeb.Models
{
    public class LoginViewModel
    {
        [Display(Name = "E-Posta")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi yazınız!")]
        public string UserName { get; set; }

        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Password { get; set; }

        [Display(Name = "Oturum açık kalsın")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
