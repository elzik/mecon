﻿namespace Elzik.Mecon.Framework.Infrastructure.Plex.Options
{
    public class PlexOptions
    {
        public string BaseUrl { get; set; }
        public string AuthToken { get; set; }
        public int ItemsPerCall { get; set; } = 100;
    }
}
