using ExpenceTracker.Shared.Domain;

namespace ExpenceTracker.Modules.Badges.Domain
{
    public class Badge : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public Badge() { }

        public Badge(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
}
