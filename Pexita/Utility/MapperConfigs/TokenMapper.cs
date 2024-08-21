using AutoMapper;
using Pexita.Data.Entities.Authentication;

namespace Pexita.Utility.MapperConfigs
{
    public class TokenMapper : Profile
    {
        public TokenMapper()
        {
            CreateMap<BrandRefreshToken, BrandRefreshTokenDTO>();
            CreateMap<UserRefreshToken, UserRefreshTokenDTO>();
        }
    }
}
