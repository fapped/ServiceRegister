using Microsoft.AspNetCore.Mvc;
using ServiceRegister.DTO;
using ServiceRegister.Extensions;
using ServiceRegister.Models;
using System.Net;
using System.Text.Json;
using static ServiceRegister.Extensions.Extensions;

namespace ServiceRegister.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        static List<ServiceInfo> services = new List<ServiceInfo>()
        {
            new ServiceInfo()
            {
                Address = new Uri("http://2.2.2.2/"),
                LastHeartBeat = DateTime.Now,
                RegisterDate = DateTime.Now,
                Name = "Nazwa"
            }
        };

        [HttpGet]
        public IEnumerable<ServiceInfo> Get()
        {
            return services;
        }

        [HttpPost]
        public IActionResult AddServiceInfo([FromBody] ServiceInfoDTO info)
        {
            var addResults = services.Add(info);

            if (addResults.Count() == 1 && addResults.First() == ValidationResult.Success)
                return Ok();

            var returnMessage = new
            {
                message = addResults.Select(res => res.ToString())
            };

            return BadRequest(returnMessage);
        }
    }
}
