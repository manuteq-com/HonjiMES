using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Hubs;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HonjiMES.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReloadController : ControllerBase
    {
        private IHubContext<ChartHub> _hubContext;
        public ReloadController(IHubContext<ChartHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            try
            {
                var message = new HubMessage();
                message.clientuniqueid = "00123";
                message.type = "sent";
                message.message = "ReloadBillboard";
                message.date = DateTime.Now;
                await _hubContext.Clients.All.SendAsync("MessageReceived", message);
            }
            catch (Exception e)
            {
                return Ok(MyFun.APIResponseError(e.Message));
            }

            return Ok(MyFun.APIResponseOK(""));
        }
    }
}
