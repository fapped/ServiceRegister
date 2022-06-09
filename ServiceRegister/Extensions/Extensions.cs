using ServiceRegister.DTO;
using ServiceRegister.Models;

namespace ServiceRegister.Extensions
{
    public static class Extensions
    {
        public enum ValidationResult
        {
            Success = 0,
            SourceNull,
            NameAlreadyExist,
            AddressAlreadyExist,
            AddressBadFormat
        }

        public static IEnumerable<ValidationResult> Add(this List<ServiceInfo> list, ServiceInfoDTO info)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (info == null)
                return Enumerable.Repeat(ValidationResult.SourceNull, 1);

            List<ValidationResult> result = new();

            if (list.Any(srv => srv.Name == info.Name))
                result.Add(ValidationResult.NameAlreadyExist);

            bool uriCorrect = Uri.TryCreate(info.Address, UriKind.Absolute, out Uri? uri);

            if (!uriCorrect || uri == null)
                result.Add(ValidationResult.AddressBadFormat);

            if (uriCorrect && uri != null && list.Any(srv => srv.Address == uri))
                result.Add(ValidationResult.AddressAlreadyExist);

            if (result.Any())
                return result;

            ServiceInfo infoToAdd = new()
            {
                Address = uri,
                Name = info.Name,
                RegisterDate = DateTime.Now,
                LastHeartBeat = null
            };

            list.Add(infoToAdd);

            return Enumerable.Repeat(ValidationResult.Success, 1);
        }
    }
}
