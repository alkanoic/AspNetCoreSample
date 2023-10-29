namespace AspNetCoreSample.Mvc.Models
{
    public class SubscribeViewModel
    {

        public string? Endpoint { get; set; }

        public string? ExpirationTime { get; set; }

        public SubscribeKey? Keys { get; set; }

        public class SubscribeKey
        {
            public string? P256dh { get; set; }

            public string? Auth { get; set; }
        }
    }
}
