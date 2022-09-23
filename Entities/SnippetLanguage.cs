using SQLite;

namespace snippet_manager.Entities
{
    public class SnippetLanguage
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Unique]
        public string? Description { get; set; }
    }
}
