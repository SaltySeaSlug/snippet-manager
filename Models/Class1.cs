using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snippet_manager.Models
{
    public class ItemValue : Snippet
    {
        public ItemKey? Key { get; set; }
        public List<string>? Keywords 
        { 
            get { return !string.IsNullOrEmpty(Keyword) ? new List<string>(Keyword.Split(new string[] { Environment.NewLine, "," }, StringSplitOptions.None).Select(p => p.Trim()).ToList()) : null; }
            set { Keyword = string.Join(", ", value ?? new()) ?? string.Empty; }
        }
        public List<string>? Imports
        {
            get { return !string.IsNullOrEmpty(Import) ? new List<string>(Import.Split(new string[] { Environment.NewLine, "," }, StringSplitOptions.None).Select(p => p.Trim()).ToList()) : null; }
            set { Import = string.Join(Environment.NewLine, value ?? new()) ?? string.Empty; }
        }
    }

    public class ItemKey : Key
    {
        public string? Group { get; set; }
        public string? Category { get; set; }
        public string? Language { get; set; }
    }

    public class SyntaxFileInfo
    {
        public string? FileName { get; set; }
        public string? DisplayName { get; set; }
        public bool IsModified { get; set; }
    }

    internal record struct Group(string? group, string? language, string? category)
    {
        public static implicit operator (string? Group, string? Language, string? Category)(Group value)
        {
            return (value.group, value.language, value.category);
        }

        public static implicit operator Group((string? group, string? language, string? category) value)
        {
            return new Group(value.group, value.language, value.category);
        }
    }
}
