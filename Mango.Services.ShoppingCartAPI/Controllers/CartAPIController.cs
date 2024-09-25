using AutoMapper;
using Azure;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDTO _responseDTO;
        private IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private IConfiguration _configuration;
        private IProductService _productService;
        private ICouponService _couponService;
        private IMessageBus? _messageBus;

        public CartAPIController(IMapper mapper, AppDbContext? appDbContext, IProductService? productService, ICouponService? couponService, IMessageBus? messageBus, IConfiguration configuration)
        {
            _responseDTO = new ResponseDTO();
            _mapper = mapper;
            _appDbContext = appDbContext;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("get-cart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_appDbContext.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_appDbContext.CartDetails
                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupons(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _responseDTO.Result = cart;
                _responseDTO.isSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDTO.isSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost("apply-coupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cartDto)
        {
            try
            {
                var cartFromDb = await _appDbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _appDbContext.CartHeaders.Update(cartFromDb);

                await _appDbContext.SaveChangesAsync();
                _responseDTO.Result = true;
                _responseDTO.isSuccess = true;

            }
            catch (Exception ex)
            {
                _responseDTO.isSuccess = false;
                _responseDTO.Message = ex.ToString();
            }
            return _responseDTO;
        }

        [HttpPost("cart-upsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeadersFromDb =
                    await _appDbContext.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);

                if (cartHeadersFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _appDbContext.CartHeaders.Add(cartHeader);
                    await _appDbContext.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _appDbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb =
                        await _appDbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                            u => u.ProductId == cartDTO.CartDetails.First().ProductId
                            && u.CartDetailsId == cartHeadersFromDb.CartHeaderId
                        );

                    if(cartDetailsFromDb == null)
                    {
                        cartDTO.CartDetails.First().CartHeaderId = cartHeadersFromDb.CartHeaderId;
                        _appDbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId += cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId += cartDetailsFromDb.CartDetailsId;

                        _appDbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                }

                _responseDTO.isSuccess = true;
                _responseDTO.Result = cartDTO;

            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.isSuccess = false;
            }

            return _responseDTO;
        }


        [HttpPost("cart-remove")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _appDbContext.CartDetails.FirstOrDefault(u => u.CartDetailsId == cartDetailsId);

                int totalCartItems = _appDbContext.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _appDbContext.CartDetails.Remove(cartDetails);

                if(totalCartItems == 1)
                {
                    var cartHeaderToRemove = 
                        await _appDbContext.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _appDbContext.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _appDbContext.SaveChangesAsync();

                _responseDTO.isSuccess = true;
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.isSuccess = false;
            }

            return _responseDTO;
        }


        [HttpPost("email-cart-request")]
        public async Task<Object> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                await _messageBus.PublishMessage(cartDTO, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _responseDTO.isSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDTO.isSuccess = false;
                _responseDTO.Message = ex.Message.ToString();
            }

            return _responseDTO;
        }
    }
}
