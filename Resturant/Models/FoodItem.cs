using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace Resturant.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodItemId { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "You did not enter a valid currency value ex. $15.6")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; }

        [Display(Name = "Image")]
        [DefaultValue("default.jpg")]
        public string Foodpic { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
