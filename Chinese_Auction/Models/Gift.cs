using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinese_Auction.Models
{
    public class Gift
    {
        [Required]
        public int Id { get; set; }
        [Required,MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        public string? Details { get; set; }

        [Required]
        public string Picture { get; set; } = string.Empty;

        [Required]
        public int Value { get; set; } = 10;
        public int Purchase_quantity { get; set; } = 0;

        [Required]
        public int Donor_Id { get; set; }

        [Required, ForeignKey("Donor_Id")]
        public Donor? Donor { get; set; } = null;

        [Required]
        public int Category_Id { get; set; }

        [Required, ForeignKey("Category_Id")]
        public Category? Category { get; set; } = null;

        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public bool IsLottery { get; set; } = false;
    }
}