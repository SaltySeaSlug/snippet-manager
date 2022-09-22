using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snippet_manager.Models
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

    public class Snippet
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public long KeyId { get; set; }
        public string? Keyword { get; set; }
        public string? Import { get; set; }
        public string? Code { get; set; }
    }

    public class SnippetCategory
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        
        [Unique] 
        public string? Description { get; set; }
    }

    public class SnippetGroup
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        
        [Unique] 
        public string? Description { get; set; }
    }

    public class SnippetLanguage
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        
        [Unique]
        public string? Description { get; set; }
    }

    public class SnippetQuery
    {
        public long Id { get; set; }
        public long SnippetGroupId { get; set; }
        public string? SnippetGroup { get; set; }
        public long SnippetCategoryId { get; set; }
        public string? SnippetCategory { get; set; }
        public long SnippetLanguageId { get; set; }
        public string? SnippetLanguage { get; set; }
        public string? Name { get; set; }
        public long? SnippetId { get; set; }
        public string? Keywords { get; set; }
        public string? Imports { get; set; }
        public string? Code { get; set; }
        public bool IsPublic { get; set; }
    }
}
