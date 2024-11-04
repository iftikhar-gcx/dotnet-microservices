using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        // To invoke the Product Service API
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // Default is Index() but renamed to productIndex as in _Layout.cshtml  asp-action
        // Also, default return type is IActionResult but since we are invoking async method,
        // we need to use async and Task<IActionResult> to support await keyword
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO>? list = new();
            ResponseDTO? response = await _productService.GetAllProductsAsync();

            if (response != null && response.isSuccess && response.Result != null)
            {
                // Deserialize the result object, but before that convert it string for conversion
                string? resultStr = response.Result.ToString();
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(resultStr);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            // To show the list in view, add a @model in view
            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductAsync(productDTO);

                if (response != null && response.isSuccess)
                {
                    TempData["success"] = "Product created successfully!";
                    // nameof(productIndex) is same as "productIndex" magic string.
                    return RedirectToAction(nameof(ProductIndex));

                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDTO);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.isSuccess)
            {

                // Deserialize the result object, but before that convert it string for conversion
                string? resultStr = response.Result.ToString();
                ProductDTO model = JsonConvert.DeserializeObject<ProductDTO>(resultStr);

                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO productDTO)
        {
            ResponseDTO? response = await _productService.DeleteProductAsync(productDTO.ProductId);

            if (response != null && response.isSuccess)
            {
                TempData["success"] = "Product deleted successfully!";

                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDTO);
        }


        public async Task<IActionResult> ProductEdit(int productId)
		{
			ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

			if (response != null && response.isSuccess)
			{

				// Deserialize the result object, but before that convert it string for conversion
				string? resultStr = response.Result.ToString();
				ProductDTO model = JsonConvert.DeserializeObject<ProductDTO>(resultStr);

				return View(model);
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDTO productDTO)
		{
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.UpdateProductAsync(productDTO);

                if (response != null && response.isSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDTO);
		}
	}
}
