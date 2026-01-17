using System.ComponentModel.DataAnnotations;

namespace Chinese_Auction.Dto_s
{
    public class CreatePackageDto
    {
        [Required, MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Cards_quantity { get; set; }

        [Required]
        public int Price { get; set; }
    }

    public class GetPackageDto
    {
        [Required]
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Cards_quantity { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
