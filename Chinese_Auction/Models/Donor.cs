using System.ComponentModel.DataAnnotations;

namespace Chinese_Auction.Models
{
    public class Donor
    {
        public int Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; } = string.Empty;
        [Required, MaxLength(30)]
        public string First_name { get; set; } = string.Empty;
        [Required, MaxLength(30)]
        public string Last_name { get; set; } = string.Empty ;
        public string? Phone { get; set; }
        [MaxLength(50)]
        public string? Company_name { get; set; }
        public string? Company_description { get; set; }
        public string? Company_picture { get; set; }
        [Required]
        public bool Is_publish { get; set; } = false;
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();
    }
}
