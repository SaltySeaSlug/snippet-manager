using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snippet_manager.Models
{
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
