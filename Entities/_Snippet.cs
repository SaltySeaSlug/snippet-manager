using SQLite;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel.DataAnnotations;

namespace snippet_manager.Entities
{
    [Table("Snippet")]
    public class _Snippet
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Required]
        [ForeignKey(typeof(_Author))]
        public long AuthorId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public _Author? Author { get; set; }

        [Required]
        [ForeignKey(typeof(_Group))]
        public long GroupId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public _Group? Group { get; set; }

        [Required]
        [ForeignKey(typeof(_Category))]
        public long CategoryId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public _Category? Category { get; set; }

        [Indexed]
        [Required]
        [ForeignKey(typeof(_Language))]
        public long LanguageId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public _Language? Language { get; set; }

        [Indexed]
        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [Required]
        [StringLength(255)]
        public string? Keyword { get; set; }

        [StringLength(255)]
        public string? Summary { get; set; }
        public string? Code { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? Updated { get; set; }

        [StringLength(8)]
        public string? Version { get; set; }
    }

    [Table("Author")]
    public class _Author
    {
        public _Author()
        {
            Snippets = new();
        }

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<_Snippet> Snippets { get; set; }
    }

    [Table("Group")]
    public class _Group
    {
        public _Group()
        {
            Snippets = new();
        }

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Unique]
        [Indexed(Name = "GroupDescription")]
        public string? Description { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<_Snippet> Snippets { get; set; }
    }

    [Table("Category")]
    public class _Category
    {
        public _Category()
        {
            Snippets = new();
        }

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Unique]
        [Indexed(Name = "CategoryDescription")]
        public string? Description { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<_Snippet> Snippets { get; set; }
    }

    [Table("Language")]
    public class _Language
    {
        public _Language()
        {
            Snippets = new();
        }

        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        [Indexed(Name = "LanguageDescription")]
        [Unique]
        public string? Description { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<_Snippet> Snippets { get; set; }
    }
}
