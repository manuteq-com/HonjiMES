using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HonjiMES.Hubs;
using HonjiMES.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
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
        public async Task<ActionResult<HubMessage>> Get()
        {
            try
            {
                var message = new HubMessage();
                message.clientuniqueid = "00123";
                message.type = "ReloadBillboard";
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