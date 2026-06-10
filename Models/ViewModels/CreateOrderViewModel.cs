using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models.ViewModels
{
    public class CreateOrderViewModel
    {
        public int? AddressId { get; set; }

        public string? NewAddress { get; set; }

        public bool SaveAddress { get; set; }

        public string? Comment { get; set; }
    }
}