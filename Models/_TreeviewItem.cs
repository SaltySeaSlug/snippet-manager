using snippet_manager.Entities;

namespace snippet_manager.Models
{
    public class _TreeviewItem
    {
        public long Id { get; set; }

        public long AuthorId { get; set; }
        public _Author? Author { get; set; }

        public long GroupId { get; set; }
        public _Group? Group { get; set; }

        public long CategoryId { get; set; }
        public _Category? Category { get; set; }

        public long LanguageId { get; set; }
        public _Language? Language { get; set; }

        public string? Title { get; set; }

        public string? Keywords { get; set; }
        public string? Summary { get; set; }
        public string? Code { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public string? Version { get; set; }
    }
    public class _ComboBoxItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }
}
