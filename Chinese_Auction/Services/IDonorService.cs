using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Services
{
    public interface IDonorService
    {
        Task<ManagerGetDonorDto> CreateDonorAsync(CreateDonorDto donor);
        Task<bool> DeleteDonorAsync(int id);
        Task<bool> DonorEmailExistsAsync(string email, int id);
        Task<IEnumerable<ManagerGetDonorDto>> GetAllDonorsAsync();
        Task<ManagerGetDonorDto?> GetDonorByEmailAsync(string email);
        Task<ManagerGetDonorDto?> GetDonorByIdAsync(int id);
        Task<ManagerGetDonorDto?> UpdateDonorAsync(int id, CreateDonorDto donor);
        Task<IEnumerable<ManagerGetDonorDto>> GetFilteredDonorsAsync(string? name, string? email, string? giftName);

    }
}