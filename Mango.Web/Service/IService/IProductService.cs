using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductsAsync();
        Task<ResponseDTO?> GetProductByIdAsync(int ProductId);
        Task<ResponseDTO?> CreateProductAsync(ProductDTO ProductDto);
        Task<ResponseDTO?> UpdateProductAsync(ProductDTO ProductDto);
        Task<ResponseDTO?> DeleteProductAsync(int ProductId);
    }
}
