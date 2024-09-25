using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		// To invoke the Product Service API
		private readonly IProductService _productService;
		private readonly ICartService _cartService;
		public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
			_productService = productService;
			_cartService = cartService;
		}

		public async Task<IActionResult> Index()
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO? product = new ProductDTO();
            ResponseDTO? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.isSuccess && response.Result != null)
            {
                // Deserialize the result object, but before that convert it string for conversion
                string? resultStr = response.Result.ToString();
                product = JsonConvert.DeserializeObject<ProductDTO>(resultStr);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            // To show the list in view, add a @model in view
            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetailsDTO = new CartDetailsDTO()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId
            };

            List<CartDetailsDTO> cartDetailsDTOs = new List<CartDetailsDTO> { cartDetailsDTO };
            cartDTO.CartDetails = cartDetailsDTOs;

            ResponseDTO? response = await _cartService.UpsertCartAsync(cartDTO);

            if (response != null && response.isSuccess && response.Result != null)
            {
                TempData["success"] = "Item has been added to cart successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            // To show the list in view, add a @model in view
            return View(productDTO);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
