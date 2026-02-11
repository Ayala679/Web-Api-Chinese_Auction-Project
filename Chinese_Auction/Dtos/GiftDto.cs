using Chinese_Auction.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinese_Auction.Dto_s
{
    public class GiftDto
    {

        [Required, MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        public string? Details { get; set; }
        [Required]
        public string Picture { get; set; } = string.Empty;
        [Required]
        public int Value { get; set; }
        [Required]
        public int Donor_Id { get; set; }
        [Required]
        public int Category_Id { get; set; } 
        public bool IsLottery { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }

    public class GetGiftDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string? Details { get; set; }
        [Required]
        public string Picture { get; set; } = string.Empty;
        [Required]
        public int Value { get; set; }
        [Required]
        public int Donor_Id { get; set; }

        public UserGetDonorDto Donor { get; set; }
        [Required]
        public string Category_Id { get; set; } = string.Empty;
        public CategoryDto Category { get; set; }
        public bool IsLottery { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        [Required]
        public int Purchase_quantity { get; set; }
    }

    public class UpdateGiftDto
    {
        public int Purchase_quantity { get; set; }
    }

    public class ApproveGiftDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool IsApproved { get; set; } = false;
    }


}
