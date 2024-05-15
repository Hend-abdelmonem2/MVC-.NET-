using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resturant.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int fooditemId { get; set; }
        public int userid { get; set; }
        public int quantity { get; set; }
        public DateTime orderdate { get; set; }
        public User? User { get; set; }
        public FoodItem? FoodItem { get; set; }
    }
}
