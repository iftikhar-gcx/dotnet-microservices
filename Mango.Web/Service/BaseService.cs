using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        // When we need to make API call, HttpClient is required.
        // With new NETCore, it is replaced with IHttpClientFactory

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBeater = true)
        {
            // Steps to create a Http request to send/receive requests.

            try
            {
                // 01. Make an HTTP client with any name i.e. MangoAPI
                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");

                // 02. Add request message and headers
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                // ToDo: Add authentication header
                if (withBeater)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                // 03. Specificy Uri for HttpRequest
                message.RequestUri = new Uri(requestDTO.Url);

                // 04. Serialize Data and Add it to request message
                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

                // 05. Request part is done. Now, start handling the response
                HttpResponseMessage? apiResonse = null;
                switch (requestDTO.ApiType)
                {
                    case Utility.SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case Utility.SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case Utility.SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // 06. Send request and get response back using async method.
                apiResonse = await client.SendAsync(message);

                // 07. We will get response but before any deserialization, check the status code
                switch (apiResonse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        return new() { isSuccess = false, Message = "Not authorized" };
                    case HttpStatusCode.Forbidden:
                        return new() { isSuccess = false, Message = "Forbidden" };
                    case HttpStatusCode.InternalServerError:
                        return new() { isSuccess = false, Message = "Internal Server Error" };
                    case HttpStatusCode.NotFound:
                        return new() { isSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.BadRequest:
                        return new() { isSuccess = false, Message = "Bad Request" };
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Accepted:
                        var apiContent = await apiResonse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);

                        return apiResponseDto;

                    default:
                        return new() { isSuccess = false, Message = apiResonse.ReasonPhrase.ToString() };
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO()
                {
                    isSuccess = false,
                    Message = ex.Message.ToString()
                };

                return dto;
            }
        }
    }
}
