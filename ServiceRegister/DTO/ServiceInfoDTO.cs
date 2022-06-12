using System.ComponentModel.DataAnnotations;

namespace ServiceRegister.DTO
{
    public static class DataRequirements
    {
        public const int MinLength = 3;
    }

    public class ServiceInfoDTO
    {
        [Required(AllowEmptyStrings = false), StringLength(int.MaxValue, MinimumLength = DataRequirements.MinLength)]
        public string Name { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false), Url]
        public string Address { get; set; } = string.Empty;
    }
}
