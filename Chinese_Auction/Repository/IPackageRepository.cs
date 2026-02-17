using Chinese_Auction.Models;

namespace Chinese_Auction.Repository
{
    public interface IPackageRepository
    {
        Task CreatePackageAsync(Package package);
        Task<IEnumerable<Package>> GetAllPackagesAsync();
        Task<Package?> GetPackageByIdAsync(int id);
        Task<int> GetPackagePriceByIdAsync(int id);
        Task<bool> PackageNameExistsAsync(string name, int id);
    }
}