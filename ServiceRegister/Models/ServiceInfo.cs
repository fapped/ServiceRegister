using ServiceRegister.DTO;

namespace ServiceRegister.Models
{
    public class ServiceInfo
    {
        public string Name { get; set; } = string.Empty;
        public Uri? Address { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastHeartBeatRequest { get; set; }
        public bool IsHealthy { get; set; } = false;
    }
}
