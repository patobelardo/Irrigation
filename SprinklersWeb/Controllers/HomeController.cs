using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SprinklersWeb.Models;

namespace SprinklersWeb.Controllers
{
    public class HomeController : Controller
    {
        const string ServiceBusConnectionString = "Endpoint=sb://irrigation.servicebus.windows.net/;SharedAccessKeyName=WebPage;SharedAccessKey=Nn/iZOzpummawkGqdO12sCw+4laYFwQmRf/4tCxlPGc=;";
        const string QueueName = "sprinklers";

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Change(int sprinklerNr, string sprinklerAction)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            string messageBody = $"Sprinkler;{sprinklerNr};{sprinklerAction}";
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            // Write the body of the message to the console
            Console.WriteLine($"Sending message: {messageBody}");

            // Send the message to the queue
            await queueClient.SendAsync(message);

            return View("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
