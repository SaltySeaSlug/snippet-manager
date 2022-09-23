using snippet_manager.Entities;

namespace snippet_manager.Models
{
    public class ItemKey : Key
    {
        public string? Group { get; set; }
        public string? Category { get; set; }
        public string? Language { get; set; }
    }
}
