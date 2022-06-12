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
                Name = "Service1"
            },
            new ServiceInfo()
            {
                Address = new Uri("http://190.190.190.190:100"),
                LastHeartBeat = null,
                RegisterDate = new DateTime(2000, 07, 23),
                Name = "Service2"
            },
            new ServiceInfo()
            {
                Address = new Uri("https://190.190.190.190:200"),
                LastHeartBeat = null,
                RegisterDate = new DateTime(2000, 07, 23),
                Name = "Service3"
            },
            new ServiceInfo()
            {
                Address = new Uri("http://190.190.190.190:300"),
                LastHeartBeat = null,
                RegisterDate = new DateTime(2000, 07, 23),
                Name = "Service4"
            }
            ,
            new ServiceInfo()
            {
                Address = new Uri("http://190.190.190.191:100"),
                LastHeartBeat = null,
                RegisterDate = new DateTime(2000, 07, 23),
                Name = "Service5"
            }
        };

        [HttpGet]
        public IEnumerable<ServiceInfo> Get()
        {
            return services;
        }

        [HttpGet("byName/{name}")]
        public ActionResult<ServiceInfo> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Name was empty" });

            var obj = services.Where(srv => srv.Name == name);

            if (obj == null || !obj.Any())
                return NotFound(new { message = $"Service named {name} not found" });

            if (obj.Count() > 1)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Found more then one service named {name}" });

            return Ok(obj.Single());
        }

        [HttpGet("byUri/{address}")]
        public ActionResult<ServiceInfo> GetByAddress(string address)
        {
            var escapedAddress = Uri.UnescapeDataString(address);

            if (!Uri.TryCreate(escapedAddress, UriKind.Absolute, out Uri? uri) || uri == null)
                return BadRequest(new { message = $"Address {escapedAddress} is not valid" });

            UriComponents searchType;

            if (uri.OriginalString.Count(ch => ch == ':') > 1)
                searchType = UriComponents.HostAndPort;
            else
                searchType = UriComponents.Host;

            var obj = services.Where(srv => Uri.Compare(srv.Address, uri, searchType, UriFormat.UriEscaped, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (obj == null || !obj.Any())
                return NotFound(new { message = $"Service with address {escapedAddress} not found" });

            return Ok(obj);
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
