using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class AddAddressViewModel
    {

        public string Address { get; set; } = string.Empty;
        public string? RecipientName { get; set; }
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }

        public bool IsForAnotherPerson { get; set; } = false;
    }
}