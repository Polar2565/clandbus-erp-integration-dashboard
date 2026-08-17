using System.Net;
using System.Text;
using System.Text.Json;
using ClandbusERPIntegration.Configurations;
using ClandbusERPIntegration.DTOs;
using ClandbusERPIntegration.Interfaces;
using Microsoft.Extensions.Options;

namespace ClandbusERPIntegration.Services
{
    public class AcumaticaService : IAcumaticaService
    {
        private readonly AcumaticaSettings _settings;

        private readonly HttpClient _httpClient;

        private bool _isLoggedIn = false;

        private LoginRequestDto? _currentSession;

        public AcumaticaService(
            HttpClient httpClient,
            IOptions<AcumaticaSettings> settings)
        {
            _settings = settings.Value;

            _httpClient = httpClient;

            _httpClient.BaseAddress =
                new Uri(_settings.BaseUrl);

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "PostmanRuntime/7.43.0");

            _httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "*/*");
        }

        public async Task<bool> LoginAsync(
            LoginRequestDto loginRequest)
        {
            if (_isLoggedIn)
            {
                return true;
            }

            var loginData = new
            {
                name = loginRequest.Username,
                password = loginRequest.Password,
                tenant = loginRequest.Company,
                branch = loginRequest.Branch
            };

            var json =
                JsonSerializer.Serialize(
                    loginData);

            var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "entity/auth/login");

            request.Version =
                HttpVersion.Version11;

            request.Content = content;

            var response =
                await _httpClient.SendAsync(
                    request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            _currentSession =
                loginRequest;

            _isLoggedIn = true;

            return true;
        }

        public async Task<List<SalesOrderDto>>
            GetLastSalesOrdersAsync()
        {
            if (!_isLoggedIn ||
                _currentSession == null)
            {
                return new List<SalesOrderDto>();
            }

            var endpoint =
                "entity/Default/24.200.001/SalesOrder";

            var response =
                await _httpClient.GetAsync(
                    endpoint);

            var json =
                await response.Content
                    .ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new List<SalesOrderDto>();
            }

            var options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            var orders =
                JsonSerializer.Deserialize
                <List<SalesOrderDto>>(
                    json,
                    options);

            return orders ??
                new List<SalesOrderDto>();
        }

        public async Task<bool> UpdateOrderAsync(
            UpdateOrderDto request)
        {
            if (!_isLoggedIn)
            {
                return false;
            }

            var updateBody = new
            {
                orderType = new
                {
                    value = request.OrderType
                },

                orderNbr = new
                {
                    value = request.OrderNbr
                },

                description = new
                {
                    value = request.Description
                }
            };

            var json =
                JsonSerializer.Serialize(
                    updateBody);

            var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            var endpoint =
                "entity/Default/24.200.001/SalesOrder";

            var response =
                await _httpClient.PutAsync(
                    endpoint,
                    content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveHoldAsync(
            RemoveHoldDto request)
        {
            if (!_isLoggedIn)
            {
                return false;
            }

            var updateBody = new
            {
                orderType = new
                {
                    value = request.OrderType
                },

                orderNbr = new
                {
                    value = request.OrderNbr
                },

                hold = new
                {
                    value = false
                }
            };

            var json =
                JsonSerializer.Serialize(
                    updateBody);

            var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            var endpoint =
                "entity/Default/24.200.001/SalesOrder";

            var response =
                await _httpClient.PutAsync(
                    endpoint,
                    content);

            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync(
                "entity/auth/logout",
                null);

            _isLoggedIn = false;

            _currentSession = null;

        }
    }
}
