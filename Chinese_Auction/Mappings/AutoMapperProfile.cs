using AutoMapper;
using Chinese_Auction.Dto_s;
using Chinese_Auction.Models;

namespace Chinese_Auction.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Category
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, GetCategoryDto>();
            CreateMap<Category, CategoryDto>();

            // Gift
            CreateMap<Gift, GiftDto>().ReverseMap();
                //.ForMember(dest => dest.Category_Name, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
              CreateMap<UpdateGiftDto, Gift>()
                .ForMember(dest => dest.Purchase_quantity, opt => opt.Condition((src, dest, srcMember) => srcMember != 0)).ReverseMap();
            CreateMap<Gift, GetGiftDto>();
           
            // Package
            CreateMap<Package, GetPackageDto>();
            CreateMap<CreatePackageDto,Package>();


            // Purchase
            CreateMap<CreatePurchaseDto, Purchase>();
            CreateMap<Purchase, GetPurchaseDto>();
            CreateMap<UpdatePurchaseDto, Purchase>();

            // Donor
            CreateMap<CreateDonorDto, Donor>();
            CreateMap<Donor, ManagerGetDonorDto>();
            CreateMap<Donor, UserGetDonorDto>();

            // User
            CreateMap<CreateUserDto, User>();
            CreateMap<User, GetUserDto>();

        }
    }
}