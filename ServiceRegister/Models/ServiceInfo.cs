﻿using System.Text.Json.Serialization;

namespace ServiceRegister.Models
{
    public delegate void Notify();  // delegate
    public class ServiceInfo : IDisposable
    {
        public static readonly int httpClientTimeout = 3;
        public static readonly int hearbeatPeriod = 5;
        public static readonly int invalidationTime = 20;

        static readonly HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(httpClientTimeout),
        };

        public ServiceInfo()
        {
            heartBeatTimer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(hearbeatPeriod));
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
        public DateTime? LastHeartBeatCorrect { get; set; }
        public bool IsHealthy => LastHeartBeatRequest == LastHeartBeatCorrect;

        [JsonIgnore]
        private Timer? heartBeatTimer = null;

        public delegate void LongNotRespondingHandler(ServiceInfo serviceInfo);
        public event LongNotRespondingHandler? LongNotResponding;

        private void TimerCallback(object? state)
        {
            HeartBeat().GetAwaiter().GetResult();
            if (LastHeartBeatRequest - LastHeartBeatCorrect > TimeSpan.FromSeconds(invalidationTime))
                LongNotResponding?.Invoke(this);
        }

        private async Task HeartBeat()
        {
            LastHeartBeatRequest = DateTime.Now;

            if (Address == null)
                return;

            Uri heartbeatEndPoint = new(Address, "heartbeat");
            try
            {
                HttpResponseMessage response = await client.GetAsync(heartbeatEndPoint);

                if (response.IsSuccessStatusCode)
                    LastHeartBeatCorrect = LastHeartBeatRequest;
            }
            catch (TaskCanceledException cancelEx)
            {
                Console.WriteLine("Task Canceled: " + cancelEx.Message);
            }
            catch (HttpRequestException reqEx)
            {
                Console.WriteLine("HTTP Request: " + reqEx.Message);
            }

            Console.WriteLine(Name + " req: " + LastHeartBeatRequest + " cor: " + LastHeartBeatCorrect + " " + IsHealthy);
        }
    }
}
