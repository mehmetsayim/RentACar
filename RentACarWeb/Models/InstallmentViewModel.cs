namespace RentACarWeb.Models
{
    public class CreditCardViewModel
    {
        public string Code { get; set; }

        public IList<InstallmentViewModel> Installments { get; set; }

    }
    public class InstallmentViewModel
    {
        public decimal Rate { get; set; }
        public bool Exists { get; set; }
    }
}
