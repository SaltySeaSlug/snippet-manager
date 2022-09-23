using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snippet_manager.Entities
{
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
