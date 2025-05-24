using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PersonalPortfolioTracker.DTOs.Request;
using PersonalPortfolioTracker.DTOs.Response;
using PersonalPortfolioTracker.Services.Interfaces;

namespace PersonalPortfolioTracker.Controllers
{
    [Route("")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IService _service;

        public Controller(IService service)
        {
            _service = service;
        }

        // GET: /healthcheck
        [HttpGet("healthcheck")]
        public ActionResult GetHealthCheck()
        {
            return new OkResult();
        }

        // GET: /positions
        [HttpGet("positions")]
        public async Task<ActionResult<IEnumerable<PositionResponseDto>>> GetAllPositions()
        {
            try
            {
                return new OkObjectResult(await _service.GetAllPositionsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PATCH: /position/1/status
        [HttpPatch("position/{id}/status")]
        public async Task<IActionResult> UpdatePositionStatus(int id, [FromBody] UpdatePositionStatusRequestDto updatePositionStatusRequestDto)
        {
            try
            {
                var operationResponseDto = await _service.UpdatePositionStatusAsync(id, updatePositionStatusRequestDto);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: /position/1
        [HttpDelete("position/{id}")]
        public async Task<ActionResult<OperationResponseDto>> DeletePosition(int id)
        {
            try
            {
                var operationResponseDto = await _service.DeletePositionAsync(id);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /transaction
        [HttpPost("transaction")]
        public async Task<ActionResult<OperationResponseDto>> AddTransaction([FromBody] AddTransactionRequestDto addTransactionRequestDto)
        {
            try
            {
                var operationResponseDto = await _service.AddTransactionAsync(addTransactionRequestDto);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: /transaction/1
        [HttpDelete("transaction/{id}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteTransaction(int id)
        {
            try
            {
                var operationResponseDto = await _service.DeleteTransactionAsync(id);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /dividend
        [HttpPost("dividend")]
        public async Task<ActionResult<OperationResponseDto>> AddDividend([FromBody] AddDividendRequestDto addDividendRequestDto)
        {
            try
            {
                var operationResponseDto = await _service.AddDividendAsync(addDividendRequestDto);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: /dividend/1
        [HttpDelete("dividend/{id}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteDividend(int id)
        {
            try
            {
                var operationResponseDto = await _service.DeleteDividendAsync(id);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /capitalchange
        [HttpPost("capitalchange")]
        public async Task<ActionResult<OperationResponseDto>> AddCapitalChange([FromBody] AddCapitalChangeRequestDto addCapitalChangeRequestDto)
        {
            try
            {
                var operationResponseDto = await _service.AddCapitalChangeAsync(addCapitalChangeRequestDto);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: /capitalchange/1
        [HttpDelete("capitalchange/{id}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteCapitalChange(int id)
        {
            try
            {
                var operationResponseDto = await _service.DeleteCapitalChangeAsync(id);
                return StatusCode(operationResponseDto.StatusCode, operationResponseDto.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}