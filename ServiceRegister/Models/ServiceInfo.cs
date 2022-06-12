using ServiceRegister.DTO;
using System.Text.Json.Serialization;

namespace ServiceRegister.Models
{
    public class ServiceInfo : IDisposable
    {
        public ServiceInfo()
        {
            heartBeatTimer = new Timer(HeartBeat, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            heartBeatTimer?.Dispose();
        }

        public string Name { get; set; } = string.Empty;
        public Uri? Address { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastHeartBeatRequest { get; set; }
        public bool IsHealthy { get; set; } = false;

        [JsonIgnore]
        private Timer? heartBeatTimer = null;

        private void HeartBeat(object? state)
        {
            LastHeartBeatRequest = DateTime.Now;
            Console.WriteLine(Name + " " + DateTime.Now);
        }
    }
}
