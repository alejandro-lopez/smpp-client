using Microsoft.AspNetCore.Mvc;
using Inetlab.SMPP;
using Microsoft.OpenApi.Any;
using Inetlab.SMPP.Common;
using Inetlab.SMPP.PDU;
using Inetlab.SMPP.Logging;

namespace SMPP_API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SmppApiController : ControllerBase
	{
		
		private readonly ILogger<SmppApiController> _logger;
		public static SmppClient _smppClient = new SmppClient();
		public static bool smppConnected = false;

		public SmppApiController(ILogger<SmppApiController> logger)
		{
			_logger = logger;
		}

		[HttpPost("ConnectSMPP")]
		public async Task<OkObjectResult> ConnectSMPP()
		{
			if (await _smppClient.ConnectAsync("smscsim.smpp.org", 2775)) {
				BindResp bindResp = await _smppClient.BindAsync("olIqu1Wslimawto", "ZRFgn99P", ConnectionMode.Transceiver);

				if (bindResp.Header.Status == CommandStatus.ESME_ROK)
				{
					smppConnected = true;
					Console.WriteLine("Bound with SMPP server");
					return Ok("Connected");
				}
				else
				{
					Console.WriteLine(bindResp.Header.Status);
					return Ok(bindResp.Header.Status);
				}
			}
			else
			{
				return Ok("Could not connect");

			}
		}

		[HttpPost("DisconnectSMPP")]
		public async void DisconnectSMPP()
		{
			await _smppClient.DisconnectAsync();
			smppConnected = false;
		}
		
		[HttpPost("SendSMS")]
		public void SendSMS(string senderName = "29282", string msisdn = "526643161751", string message = "Hello World!")
		{
			if(smppConnected)
			{
				IList<SubmitSm> pduList = SMS.ForSubmit().From(senderName).To(msisdn).Text(message).Create(_smppClient);
				foreach (var pdu in pduList)
				{
					Console.WriteLine("SMS Sent! ");
					Console.Write(pdu.ToString());
				}
			}
			else
			{
				Console.WriteLine("ERROR! Connect to server first before sending SMS.");
			}
		}
	}
}