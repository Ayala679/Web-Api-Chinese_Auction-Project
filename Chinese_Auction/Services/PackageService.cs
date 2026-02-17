using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;

namespace Chinese_Auction.Services
{
    public class PackageService : IPackageService
    {
        private readonly IMapper _mapper;
        private readonly IPackageRepository _packageRepository;
        private readonly ILogger<PackageService> _logger;

        public PackageService(IMapper mapper, IPackageRepository packageRepository, ILogger<PackageService> logger)
        {
            _mapper = mapper;
            _packageRepository = packageRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<GetPackageDto>> GetAllPackagesAsync()
        {
            var packages = await _packageRepository.GetAllPackagesAsync();
            return _mapper.Map<IEnumerable<GetPackageDto>>(packages);
        }

        public async Task<GetPackageDto?> GetPackageByIdAsync(int id)
        {
            var package = await _packageRepository.GetPackageByIdAsync(id);
            if (package == null)
            {
                _logger.LogWarning("Package with ID {PackageId} not found.", id);
                return null;
            }

            return _mapper.Map<GetPackageDto>(package);
        }

        public async Task<GetPackageDto> CreatePackageAsync(CreatePackageDto createPackageDto)
        {
            var package = _mapper.Map<Package>(createPackageDto);
            await _packageRepository.CreatePackageAsync(package);
            return _mapper.Map<GetPackageDto>(package);
        }







    }
}
