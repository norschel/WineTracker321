using System;

namespace WineTracker.Models
{
    public class Wine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Rate { get; set; } = 1;
        public string RateDescription { get; set; } = string.Empty;
        public string Wineyard { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
