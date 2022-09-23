using SQLite;

namespace snippet_manager.Entities
{
    public class Key
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public long GroupId { get; set; }
        public long CategoryId { get; set; }
        public long LanguageId { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
    }
}
