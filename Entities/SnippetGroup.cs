using SQLite;

namespace snippet_manager.Entities
{
    public class SnippetGroup
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Unique]
        public string? Description { get; set; }
    }
}
