using ServiceRegister.DTO;
using ServiceRegister.Models;

namespace ServiceRegister.Helpers
{
    public class ServiceCollector
    {
        public enum ValidationResult
        {
            Success = 0,
            SourceNull,
            NameEmpty,
            NameTooShort,
            NameAlreadyExist,
            AddressAlreadyExist,
            AddressBadFormat,
            AlreadyExist
        }

        private readonly List<ServiceInfo> services = new List<ServiceInfo>();
        //{
        //    //new ServiceInfo()
        //    //{
        //    //    Address = new Uri("http://2.2.2.2/"),
        //    //    LastHeartBeatRequest = null,
        //    //    RegisterDate = DateTime.Now,
        //    //    Name = "Service1"
        //    //},
        //    //new ServiceInfo()
        //    //{
        //    //    Address = new Uri("http://190.190.190.190:100"),
        //    //    LastHeartBeatRequest = null,
        //    //    RegisterDate = new DateTime(2000, 07, 23),
        //    //    Name = "Service2"
        //    //},
        //    //new ServiceInfo()
        //    //{
        //    //    Address = new Uri("https://190.190.190.190:200"),
        //    //    LastHeartBeatRequest = null,
        //    //    RegisterDate = new DateTime(2000, 07, 23),
        //    //    Name = "Service3"
        //    //},
        //    //new ServiceInfo()
        //    //{
        //    //    Address = new Uri("http://190.190.190.190:300"),
        //    //    LastHeartBeatRequest = null,
        //    //    RegisterDate = new DateTime(2000, 07, 23),
        //    //    Name = "Service4"
        //    //}
        //    //,
        //    //new ServiceInfo()
        //    //{
        //    //    Address = new Uri("http://190.190.190.191:100"),
        //    //    LastHeartBeatRequest = null,
        //    //    RegisterDate = new DateTime(2000, 07, 23),
        //    //    Name = "Service5"
        //    //}
        //};

        public IReadOnlyCollection<ServiceInfo> Services()
        {
            return services.AsReadOnly();
        }

        public IEnumerable<ValidationResult> Add(ServiceInfoDTO info)
        {
            if (info == null)
                return Enumerable.Repeat(ValidationResult.SourceNull, 1);

            List<ValidationResult> result = new();

            var exist = services.FirstOrDefault(srv => srv.Equals(info));

            if (exist != null)
            {
                exist.Update();
                result.Add(ValidationResult.AlreadyExist);
                return result;
            }

            if (string.IsNullOrWhiteSpace(info.Name))
                result.Add(ValidationResult.NameEmpty);
            else if (info.Name.Length < DataRequirements.MinLength)
                result.Add(ValidationResult.NameTooShort);

            if (services.Any(srv => srv.Name == info.Name))
                result.Add(ValidationResult.NameAlreadyExist);

            bool uriCorrect = Uri.TryCreate(info.Address, UriKind.Absolute, out Uri? uri);

            if (!uriCorrect || uri == null)
                result.Add(ValidationResult.AddressBadFormat);

            if (uriCorrect && uri != null && services.Any(srv => srv.Address == uri))
                result.Add(ValidationResult.AddressAlreadyExist);

            if (result.Any())
                return result;

            ServiceInfo infoToAdd = new()
            {
                Address = uri,
                Name = info.Name,
                RegisterDate = DateTime.Now,
                LastHeartBeatRequest = null
            };

            infoToAdd.LongNotResponding += InfoToAdd_LongNotResponding;

            services.Add(infoToAdd);

#if DEBUG
            Console.WriteLine($"{infoToAdd.Name} {infoToAdd.Address} added!");
#endif

            return Enumerable.Repeat(ValidationResult.Success, 1);
        }

        private void InfoToAdd_LongNotResponding(ServiceInfo serviceInfo)
        {
            Console.WriteLine($"{DateTime.Now} Removing {serviceInfo.Name} - no reponse since {serviceInfo.LastHeartBeatCorrect} with timeout {ServiceInfo.invalidationTime}");
            Remove(serviceInfo);
        }

        public bool Remove(ServiceInfo info)
        {
            if (info is null)
                return false;

            info.Dispose();
            return services.Remove(info);
        }
    }
}
