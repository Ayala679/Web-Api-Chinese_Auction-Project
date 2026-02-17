using Chinese_Auction.Data;
using Chinese_Auction.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinese_Auction.Repository
{
    public class PackageRepository : IPackageRepository
    {
        private readonly ChineseAuctionDbContext _context;
        public PackageRepository(ChineseAuctionDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages.ToListAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int id)
        {
            return await _context.Packages.FindAsync(id);
        }

        //manager only
        public async Task CreatePackageAsync(Package package)
        {
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> PackageNameExistsAsync(string name, int id)
        {
            var packages = await _context.Packages.ToListAsync();
            return packages.Any(p => p.Name.Equals(name) && p.Id != id);
        }

        public async Task<int> GetPackagePriceByIdAsync(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                throw new Exception("Package not found.");
            }
            return package.Price;
        }
    }
}
