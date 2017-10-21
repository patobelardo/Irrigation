using System;
using Microsoft.AspNetCore.Mvc;
using Bifrost.Devices.Gpio.Core;
using Bifrost.Devices.Gpio.Abstractions;
using Bifrost.Devices.Gpio;
using System.Threading.Tasks;

namespace GpioSwitcherWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PinsController : Controller
    {
        private IGpioController gpioController;

        public PinsController()
        {
            Console.WriteLine("In controller - instantiating GpioController instance");
            gpioController = GpioController.Instance;
        }

        // GET api/pins
        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine("About to list pin statuses.");
            return Ok(gpioController.Pins);
        }

        // GET api/pins/5
        [HttpGet("{pinId}")]
        public IActionResult Get(int pinId)
        {
            GpioPinValue pinStatus;

            Console.WriteLine("About to get pin status.");
            var pin = gpioController.OpenPin(pinId);

            pinStatus = pin.Read();

            Console.WriteLine("Returning pin status.");
            return Ok(pinStatus.ToString());
        }

        // POST api/pins
        [HttpPost]
        public void SwitchPin(int pinId, int status, int timeout = 0)
        {
            Console.WriteLine("About to change pin status.");
            var pin = gpioController.OpenPin(pinId);

            pin.SetDriveMode(GpioPinDriveMode.Output);

            //This is turn off
            if (status == 1)
            {
                Console.WriteLine("Going off");
                pin.Write(GpioPinValue.High);
            }
            else
            {
                //Turn On
                if (timeout==0)
                    timeout = 5000;
                Console.WriteLine($"Going on: Timeout of {timeout}");
                pin.Write(GpioPinValue.Low);
                Task.Delay(timeout).Wait();
                Console.WriteLine($"Going off by timeout");
                pin.Write(GpioPinValue.High);
            }
        }
    }
}