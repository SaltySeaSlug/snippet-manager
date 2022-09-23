using snippet_manager.Entities;

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
}
