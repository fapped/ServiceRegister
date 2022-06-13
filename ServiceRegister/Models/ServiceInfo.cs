using System.Text.Json.Serialization;

namespace ServiceRegister.Models
{
    public class ServiceInfo : IDisposable
    {
        static readonly HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(3)
        };

        public ServiceInfo()
        {
            heartBeatTimer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            heartBeatTimer?.Dispose();
            GC.SuppressFinalize(true);
        }

        public string Name { get; set; } = string.Empty;
        public Uri? Address { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastHeartBeatRequest { get; set; }
        public bool IsHealthy { get; set; } = false;

        [JsonIgnore]
        private Timer? heartBeatTimer = null;

        private void TimerCallback(object? state)
        {
            HeartBeat().GetAwaiter().GetResult();
        }

        private async Task HeartBeat()
        {
            if (Address == null)
            {
                IsHealthy = false;
                return;
            }

            LastHeartBeatRequest = DateTime.Now;

            Uri heartbeatEndPoint = new(Address, "heartbeat");
            try
            {
                HttpResponseMessage response = await client.GetAsync(heartbeatEndPoint);

                if (response.IsSuccessStatusCode)
                    IsHealthy = true;
                else
                    IsHealthy = false;
            }
            catch (TaskCanceledException cancelEx)
            {
                Console.WriteLine("Task Canceled: " + cancelEx.Message);
                IsHealthy = false;
            }
            catch (HttpRequestException reqEx)
            {
                Console.WriteLine("HTTP Request: " + reqEx.Message);
                IsHealthy = false;
            }

            Console.WriteLine(Name + " " + LastHeartBeatRequest + " " + IsHealthy);
        }
    }
}
