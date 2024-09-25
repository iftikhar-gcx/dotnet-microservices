using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    // Using   [Route("api/[controller]")]   is not recommended. 
    // It uses implicit structure so mapping can be an issue in some cases
    // If you rename the controller, it will directly take effects but will also cause issue on client-side
    // Can easily cause route conflicts. Like ProductController and ProductsController


    // Always use static controller name to be used by API
    // and use the same path in API calls
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;  // DbContent
        private readonly ResponseDTO _response;    // ResponseDTO object to transfer data
        private readonly IMapper _mapper;

        public ProductAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _response = new ResponseDTO()
            {
                isSuccess = true,
                Message = "Success"
            };


            // Assignment won't work if fields/properties are different in model and DTO
            // Or assigning properties manually is not a good practice.
            // Therefore, we will implement Mapping in a custom class called "MappingConfig" in project
            // After mapping, we need to register the mapping in project.cs using IMapper
            // Also, we will need to create and pass private IMapper

            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                IEnumerable<Product> objList = _appDbContext.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDTO>>(objList);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDTO Get(int id)
        {
            try
            {
                Product obj= _appDbContext.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPost]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDTO Post([FromBody] ProductDTO ProductDTO)
        {
            try
            {
                Product obj = _mapper.Map<Product>(ProductDTO);
                _appDbContext.Products.Add(obj);
                _appDbContext.SaveChanges();

                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDTO Put([FromBody] ProductDTO ProductDTO)
        {
            try
            {
                Product obj = _mapper.Map<Product>(ProductDTO);
                _appDbContext.Products.Update(obj);
                _appDbContext.SaveChanges();

                _response.Result = _mapper.Map<ProductDTO>(obj);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                Product obj = _appDbContext.Products.First(u => u.ProductId == id);
                _appDbContext.Products.Remove(obj);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }
    }
}
