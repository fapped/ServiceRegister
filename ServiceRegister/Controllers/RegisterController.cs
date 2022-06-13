using Microsoft.AspNetCore.Mvc;
using ServiceRegister.DTO;
using ServiceRegister.Helpers;
using ServiceRegister.Models;
using static ServiceRegister.Helpers.ServiceCollector;

namespace ServiceRegister.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        static ServiceCollector serviceCollector = new();
        IReadOnlyCollection<ServiceInfo> services => serviceCollector.Services();

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
            var addResults = serviceCollector.Add(info);

            if (addResults.Count() == 1 && addResults.First() == ValidationResult.Success)
                return Ok();

            var returnMessage = new
            {
                message = addResults.Select(res => res.ToString())
            };

            return BadRequest(returnMessage);
        }

        [HttpDelete("{name}")]
        public IActionResult Unregister(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Name was empty" });

            var serviceCount = services.Where(srv => srv.Name == name).ToList();

            if (serviceCount.Count == 0)
                return NotFound(new { message = $"Service name {name} not found" });

            if (serviceCount.Count > 1)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Found more then one service named {name}" });

            var objToDelete = serviceCount.Single();
            serviceCollector.Remove(objToDelete);

            return Ok();
        }
    }
}
