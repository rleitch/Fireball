using System;

namespace Fireball.Client.Configuration
{
    public interface IFireballClientSettings
    {
        Uri BaseAddress { get; set; }
    }
}