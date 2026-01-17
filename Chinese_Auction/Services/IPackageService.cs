using Chinese_Auction.Dto_s;

namespace Chinese_Auction.Services
{
    public interface IPackageService
    {
        Task<GetPackageDto> CreatePackageAsync(CreatePackageDto createPackageDto);
        Task<bool> DeletePackageAsync(int id);
        Task<IEnumerable<GetPackageDto>> GetAllPackagesAsync();
        Task<GetPackageDto?> GetPackageByIdAsync(int id);
        Task<GetPackageDto?> UpdatePackageAsync(int id, CreatePackageDto updatePackageDto);
    }
}