using SQLite;

namespace snippet_manager.Entities
{
    public class Snippet
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long KeyId { get; set; }
        public string? Keyword { get; set; }
        public string? Import { get; set; }
        public string? Code { get; set; }
    }
}
