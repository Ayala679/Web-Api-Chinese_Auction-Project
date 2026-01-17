using System.ComponentModel.DataAnnotations;

namespace Chinese_Auction.Models
{
    public enum Role
    {
        Manager = 0,
        Customer = 1,
    }

    public class User
    {
        public int Id { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; } = string.Empty;
        [Required,MaxLength(30)]
        public string First_name { get; set; } = string.Empty;
        [Required,MaxLength(30)]
        public string Last_name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        [Required]
        public Role Role { get; set; } = Role.Customer;
        
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();


    }
}
