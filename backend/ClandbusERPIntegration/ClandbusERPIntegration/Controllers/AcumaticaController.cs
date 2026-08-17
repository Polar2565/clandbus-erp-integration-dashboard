using ClandbusERPIntegration.DTOs;
using ClandbusERPIntegration.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClandbusERPIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcumaticaController : ControllerBase
    {
        private readonly IAcumaticaService _acumaticaService;

        public AcumaticaController(
            IAcumaticaService acumaticaService)
        {
            _acumaticaService = acumaticaService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequestDto request)
        {
            try
            {
                var success =
                    await _acumaticaService
                        .LoginAsync(request);

                if (!success)
                {
                    return BadRequest(new
                    {
                        message = "ERP login failed"
                    });
                }

                return Ok(new
                {
                    message = "ERP session started successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Unexpected integration error"
                });
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders =
                    await _acumaticaService
                        .GetLastSalesOrdersAsync();

                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Unexpected integration error"
                });
            }
        }

        [HttpPost("update-order")]
        public async Task<IActionResult> UpdateOrder(
            UpdateOrderDto request)
        {
            try
            {
                var success =
                    await _acumaticaService
                        .UpdateOrderAsync(request);

                if (!success)
                {
                    return BadRequest(new
                    {
                        message = "Order update failed"
                    });
                }

                return Ok(new
                {
                    message = "Order updated successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Unexpected integration error"
                });
            }
        }

        [HttpPost("remove-hold")]
        public async Task<IActionResult> RemoveHold(
            RemoveHoldDto request)
        {
            try
            {
                var success =
                    await _acumaticaService
                        .RemoveHoldAsync(request);

                if (!success)
                {
                    return BadRequest(new
                    {
                        message = "Remove Hold failed"
                    });
                }

                return Ok(new
                {
                    message =
                        "Remove Hold executed successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Unexpected integration error"
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _acumaticaService
                    .LogoutAsync();

                return Ok(new
                {
                    message = "Logout successful"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Unexpected integration error"
                });
            }
        }
    }
}
