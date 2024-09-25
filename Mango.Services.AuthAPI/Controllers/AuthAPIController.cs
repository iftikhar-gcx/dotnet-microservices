using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {

        private readonly IAuthService _authService;
        protected ResponseDTO _response;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _response = new ResponseDTO();
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO requestDTO)
        {
            var errorMessage = await _authService.Register(requestDTO);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.isSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            _messageBus.PublishMessage(requestDTO.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO)
        {
            var loginResponse = await _authService.Login(requestDTO);
            if(loginResponse.User == null)
            {
                _response.isSuccess = false;
                _response.Message = "Username or password is incorrect";
                return Unauthorized(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO requestDTO)
        {
            var roleAssignment = await _authService.AssignRole(requestDTO.Email, requestDTO.Role.ToUpper());
            if (!roleAssignment)
            {
                _response.isSuccess = false;
                _response.Message = "Error encountered";
                return Unauthorized(_response);
            }

            _response.Result = roleAssignment;
            return Ok(_response);
        }
    }
}
