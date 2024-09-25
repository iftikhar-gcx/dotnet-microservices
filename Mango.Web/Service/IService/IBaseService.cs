using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
        // A generic method will be used to make API calls.
        // Example, Delete and GetById takes integer as id but Post takes whole object
        // Thus it's better to create a generic method instead of creating all and duplicating code.
        public Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true);
    }
}
