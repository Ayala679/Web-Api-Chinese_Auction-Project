using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;
using Chinese_Auction.Repository;

namespace Chinese_Auction.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DonorRepository> _logger;

        public DonorService(IDonorRepository donorRepository, IMapper mapper, ILogger<DonorRepository> logger)
        {
            _donorRepository = donorRepository;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<IEnumerable<ManagerGetDonorDto>> GetAllDonorsAsync()
        {
            var donors = await _donorRepository.GetAllDonorsAsync();
            return _mapper.Map<IEnumerable<ManagerGetDonorDto>>(donors);
        }


        public async Task<ManagerGetDonorDto?> GetDonorByIdAsync(int id)
        {
            var donor = await _donorRepository.GetDonorByIdAsync(id);
            if(donor == null)
            {
                _logger.LogWarning($"Donor with ID {id} not found.");
                return null;
            }
            return _mapper.Map<ManagerGetDonorDto>(donor);
        }

        public async Task<ManagerGetDonorDto> CreateDonorAsync(CreateDonorDto donor)
        {
            if (await DonorEmailExistsAsync(donor.Email, -1))
            {
                _logger.LogWarning($"Attempt to create donor with existing email: {donor.Email}");
                throw new Exception("Donor with the same email already exists.");
            }
            var createDonor = _mapper.Map<Donor>(donor);
            await _donorRepository.CreateDonorAsync(createDonor);
            return _mapper.Map<ManagerGetDonorDto>(createDonor);
        }

        public async Task<ManagerGetDonorDto?> UpdateDonorAsync(int id, CreateDonorDto donor)
        {
            var existingDonor = await _donorRepository.GetDonorByIdAsync(id);
            if (existingDonor == null) return null;
            if (existingDonor.Gifts.Any() || existingDonor.Gifts.Count() > 0)
                throw new InvalidOperationException("לא ניתן לערוך תורם שכבר תרם מתנות");
            _mapper.Map(donor, existingDonor);
            if (donor.Password != null)
            {
                existingDonor.Password = HashPassword(donor.Password);
            }
            existingDonor.Id = id;
            if (donor.Email != existingDonor.Email && donor.Email != null)
            {
                if (await DonorEmailExistsAsync(donor.Email, id))
                {
                    _logger.LogWarning($"Attempt to update donor with existing email: {donor.Email} for ID: {id}");
                    throw new Exception("Donor with the same email already exists.");
                }
            }
            var updatedDonor = await _donorRepository.UpdateDonorAsync(existingDonor);
            if (updatedDonor == null)
            {
                _logger.LogError($"Failed to update donor with ID: {id}");
                return null;
            }
            return _mapper.Map<ManagerGetDonorDto>(updatedDonor);
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }


        public async Task<bool> DeleteDonorAsync(int id)
        {
            var existingDonor = await _donorRepository.GetDonorByIdAsync(id);
            if (existingDonor == null)
            {
                _logger.LogWarning($"Attempt to delete non-existing donor with ID: {id}");
                return false;
            }
            if (existingDonor.Gifts.Any() || existingDonor.Gifts.Count() > 0)
                throw new InvalidOperationException("לא ניתן למחוק תורם שכבר תרם מתנות");
            await _donorRepository.DeleteDonorAsync(id);
            return true;
        }

        public async Task<bool> DonorEmailExistsAsync(string email, int id)
        {
            return await _donorRepository.DonorEmailExistsAsync(email, id);
        }


        public async Task<ManagerGetDonorDto?> GetDonorByEmailAsync(string email)
        {
            var donor = await _donorRepository.GetDonorByEmailAsync(email);
            if (donor == null)
            {
                _logger.LogWarning($"Donor with Email {email} not found.");
                return null;
            }
            return _mapper.Map<ManagerGetDonorDto>(donor);
        }

        public async Task<IEnumerable<ManagerGetDonorDto>> GetFilteredDonorsAsync(string? name, string? email, string? giftName)
        {
            var donors = await _donorRepository.GetFilteredDonorsAsync(name, email, giftName);
            return _mapper.Map<IEnumerable<ManagerGetDonorDto>>(donors);
        }


    }
}
