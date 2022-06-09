using System.ComponentModel.DataAnnotations;

namespace ServiceRegister.DTO
{
    public class ServiceInfoDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; } = string.Empty;
    }
}
