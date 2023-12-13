using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;

namespace AutoyVaro.Controllers
{
    public class SmsController : TwilioController
    {
        // GET: Sms
        public ActionResult Index()
        {
            var response = new VoiceResponse();
            response.Say("hello world");
            return TwiML(response);
        }
        public ActionResult Send() {
            string accountSid = "2345";
            string authToken = "2345";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Join Earth's mightiest heroes. Like Kevin Bacon.",
                from: new Twilio.Types.PhoneNumber("+w2345"),
                to: new Twilio.Types.PhoneNumber("+2343245")
            );
            return Content("");
        }
    }
}