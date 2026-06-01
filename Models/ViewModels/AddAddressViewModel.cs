namespace FlowerShop.Models.ViewModels
{
    public class AddAddressViewModel
    {
        public string Address { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }
    }
}