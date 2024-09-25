using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        // To invoke the Base service
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService) 
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = ProductAPIBase + "/api/product"
            });
        }

        public async Task<ResponseDTO?> GetProductByIdAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.GET,
                Url = ProductAPIBase + "/api/product/" + ProductId
            });
        }

        public async Task<ResponseDTO?> CreateProductAsync(ProductDTO ProductDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.POST,
                Data = ProductDto,
                Url = ProductAPIBase + "/api/product/"
            });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO ProductDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.PUT,
                Data = ProductDto,
                Url = ProductAPIBase + "/api/product/"
            });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int ProductId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = ApiType.DELETE,
                Url = ProductAPIBase + "/api/product/" + ProductId
            });
        }
	}
}
