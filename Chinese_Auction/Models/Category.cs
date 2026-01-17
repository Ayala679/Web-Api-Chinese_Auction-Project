using System.ComponentModel.DataAnnotations;

namespace Chinese_Auction.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
    } 
}
