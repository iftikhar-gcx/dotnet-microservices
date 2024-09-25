using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
                _baseService = baseService;
        }

        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registerRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = registerRequestDTO,
                Url = AuthAPIBase + "/api/auth/assign-role"
            }, withBearer: false);
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDTO,
                Url = AuthAPIBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registerRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = registerRequestDTO,
                Url = AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
