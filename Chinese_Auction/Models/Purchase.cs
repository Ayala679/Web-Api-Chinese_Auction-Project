using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinese_Auction.Models
{
    public class Purchase
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int Gift_Id { get; set; }

        [Required,ForeignKey("Gift_Id")]
        public Gift? Gift { get; set; } = null;

        [Required]
        public int User_Id { get; set; }

        [Required,ForeignKey("User_Id")]
        public User? User { get; set; } = null;

        [Required]
        public int Package_Id {  get; set; }
        [Required, ForeignKey("Package_Id")]
        public Package? Package { get; set; } = null;

        [Required]
        public string Unique_Package_Id { get; set; } = string.Empty;

        [Required]
        public DateTime Purchase_Date { get; set; } = DateTime.Now;

        public bool Is_Won { get; set; } = false;
    }
}
