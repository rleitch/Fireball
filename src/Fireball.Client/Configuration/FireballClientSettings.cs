using System;

namespace Fireball.Client.Configuration
{
    public class FireballClientSettings : IFireballClientSettings
    {
        public Uri BaseAddress { get; set; }

        public string ApiKey { get; set; }
    }
}