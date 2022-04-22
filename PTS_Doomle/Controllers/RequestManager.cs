using Microsoft.AspNetCore.Mvc;
using PTS_Doomle.Data.Responses;
using PTS_Doomle.Response.Interfaces;

namespace PTS_Doomle.Controllers
{
    [ApiController]
    [Route("api")]
    public class RequestManagerController : ControllerBase
    {
        private readonly IResponseManager responseManager;

        public RequestManagerController(IResponseManager responseManager)
        {
            this.responseManager = responseManager;
        }

        [HttpGet("{data}")]
        public IActionResult HandleRequest(string data)
        {
            switch (data)
            {
                case "frequency":
                    return this.SendResponse<FrequencyResponse>();
                case "centralTrend":
                    return this.SendResponse<CentralTrendResponse>();
                case "distraction":
                    return this.SendResponse<DistractionResponse>();
                case "correlation":
                    return this.SendResponse<CorrelationResponse>();
                default:
                    return BadRequest();
            }
        }
        
        public IActionResult SendResponse<T>()
        {
            return Content(this.responseManager.GenerateResponse<T>());
        }
    }
}
