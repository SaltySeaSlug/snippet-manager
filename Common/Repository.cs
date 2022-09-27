using snippet_manager.Entities;
using snippet_manager.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace snippet_manager.Common
{
    public class Repository
    {
        private readonly SQLiteConnection db;

        public Repository(string databaseName = "db.sqlite")
        {
            db = InitializeLocalDatabase(db, databaseName, 
                typeof(_Author), 
                typeof(_Category), 
                typeof(_Group), 
                typeof(_Language), 
                typeof(_Snippet));
        }


        public List<_Author> GetAuthors()
        {
            return db.Table<_Author>().ToList();
        }
        public _Author GetAuthorById(long id)
        {
            return db.Find<_Author>(id);
        }
        public long? Insert(_Author author)
        {
            return db.Insert(author) > 0 ? GetLastInsertId(db) : null;
        }
        public bool Update(_Author author)
        {
            var dbAuthor = GetAuthorById(author.Id);

            if (dbAuthor == null)
            {
                return false;
            }

            dbAuthor.Name = author.Name;
            dbAuthor.Surname = author.Surname;

            if (!dbAuthor.Id.Equals(author.Id))
            {
                author.Id = dbAuthor.Id;
            }

            return db.Update(dbAuthor) > 0;
        }
        public bool Delete(_Author author)
        {
            return db.Delete(author) > 0;
        }
        public bool DeleteAuthorById(long id)
        {
            return db.Delete<_Author>(id) > 0;
        }
        public long? InsertOrUpdate(_Author author)
        {
            if (!Update(author))
                return Insert(author);
            else
                return author.Id;
        }


        public List<_Category> GetCategories()
        {
            return db.Table<_Category>().ToList();
        }
        public _Category GetCategoryById(long id)
        {
            return db.Find<_Category>(id);
        }
        public long? Insert(_Category category)
        {
            return db.Insert(category) > 0 ? GetLastInsertId(db) : null;
        }
        public bool Update(_Category category)
        {
            var dbCategory = GetCategoryById(category.Id);

            if (dbCategory == null)
            {
                return false;
            }

            dbCategory.Description = category.Description;

            if (!dbCategory.Id.Equals(category.Id))
            {
                category.Id = dbCategory.Id;
            }

            return db.Update(dbCategory) > 0;
        }
        public bool Delete(_Category category)
        {
            return db.Delete(category) > 0;
        }
        public bool DeleteCategoryById(long id)
        {
            return db.Delete<_Category>(id) > 0;
        }
        public long? InsertOrUpdate(_Category category)
        {
            if (!Update(category))
                return Insert(category);
            else
                return category.Id;
        }


        public List<_Group> GetGroups()
        {
            return db.Table<_Group>().ToList();
        }
        public _Group GetGroupById(long id)
        {
            return db.Find<_Group>(id);
        }
        public _Group GetGroupByDescription(string description)
        {
            return db.Find<_Group>(_ => _.Description == description);
        }
        public long? Insert(_Group group)
        {
            return db.Insert(group) > 0 ? GetLastInsertId(db) : null;
        }
        public bool Update(_Group group)
        {
            var dbGroup = GetGroupById(group.Id) ?? GetGroupByDescription(group.Description);

            if (dbGroup == null)
            {
                return false;
            }

            dbGroup.Description = group.Description;

            if (!dbGroup.Id.Equals(group.Id))
            {
                group.Id = dbGroup.Id;
            }

            return db.Update(dbGroup) > 0;
        }
        public bool Delete(_Group group)
        {
            return db.Delete(group) > 0;
        }
        public bool DeleteGroupById(long id)
        {
            return db.Delete<_Group>(id) > 0;
        }
        public long? InsertOrUpdate(_Group group)
        {
            if (!Update(group))
                return Insert(group);
            else
                return group.Id;
        }


        public List<_Language> GetLanguages()
        {
            return db.Table<_Language>().ToList();
        }
        public _Language GetLanguageById(long id)
        {
            return db.Find<_Language>(id);
        }
        public long? Insert(_Language language)
        {
            return db.Insert(language) > 0 ? GetLastInsertId(db) : null;
        }
        public bool Update(_Language language)
        {
            var dbLanguage = GetGroupById(language.Id);

            if (dbLanguage == null)
            {
                return false;
            }

            dbLanguage.Description = language.Description;

            if (!dbLanguage.Id.Equals(language.Id))
            {
                language.Id = dbLanguage.Id;
            }

            return db.Update(dbLanguage) > 0;
        }
        public bool Delete(_Language language)
        {
            return db.Delete(language) > 0;
        }
        public bool DeleteLanguageById(long id)
        {
            return db.Delete<_Language>(id) > 0;
        }
        public long? InsertOrUpdate(_Language language)
        {
            if (!Update(language))
                return Insert(language);
            else
                return language.Id;
        }


        public List<_Snippet> GetSnippets()
        {
            return db.GetAllWithChildren<_Snippet>(recursive: true).ToList();
        }
        public _Snippet GetSnippetById(long id)
        {
            try
            {
                return db.GetWithChildren<_Snippet>(id, recursive: true);
            }
            catch { return null; }
        }
        public long? Insert(_Snippet snippet)
        {
            snippet.Created = DateTime.Now;
            return db.Insert(snippet) > 0 ? GetLastInsertId(db) : null;
        }
        public bool Update(_Snippet snippet)
        {
            var dbSnippet = GetSnippetById(snippet.Id);

            if (dbSnippet == null)
            {
                return false;
            }
            
            dbSnippet.Author = snippet.Author;
            dbSnippet.AuthorId = snippet.AuthorId;
            dbSnippet.CategoryId = snippet.CategoryId;
            dbSnippet.Category = snippet.Category;
            dbSnippet.GroupId = snippet.GroupId;
            dbSnippet.Group = snippet.Group;
            dbSnippet.LanguageId = snippet.LanguageId;
            dbSnippet.Language = snippet.Language;
            dbSnippet.Title = snippet.Title;
            dbSnippet.Summary = snippet.Summary;
            dbSnippet.Keyword = snippet.Keyword;
            dbSnippet.Code = snippet.Code;
            dbSnippet.Created = snippet.Created;
            dbSnippet.Updated = DateTime.Now;
            dbSnippet.Version = snippet.Version;

            if (!dbSnippet.Id.Equals(snippet.Id))
            {
                snippet.Id = dbSnippet.Id;
            }

            return db.Update(dbSnippet) > 0;
        }
        public bool Delete(_Snippet snippet)
        {
            return db.Delete(snippet) > 0;
        }
        public bool DeleteSnippetById(long id)
        {
            return db.Delete<_Snippet>(id) > 0;
        }
        public long? InsertOrUpdate(_Snippet snippet)
        {
            if (!Update(snippet))
                return Insert(snippet);
            else
                return snippet.Id;
        }

        public long? InsertOrUpdate(_TreeviewItem snippet)
        {
            var dbSnippet = new _Snippet
            {
                Id = snippet.Id,
                AuthorId = snippet.AuthorId,
                CategoryId = snippet.CategoryId,
                Code = snippet.Code,
                Created = snippet.Created,
                GroupId = snippet.GroupId,
                Summary = snippet.Summary,
                Keyword = snippet.Keywords,
                LanguageId = snippet.LanguageId,
                Title = snippet.Title,
                Updated = snippet.Updated,
                Version = snippet.Version
            };

            return InsertOrUpdate(dbSnippet);
        }





















        public void Close()
        {
            db.Close();
            db.Dispose();
        }



        public static long GetLastInsertId(SQLiteConnection db)
        {
            return SQLite3.LastInsertRowid(db.Handle);
        }



        public static SQLiteConnection InitializeLocalDatabase(SQLiteConnection database, string databaseName, params Type[] tables)
        {
            if (database == null)
            {
                if (!Directory.Exists(Application.StartupPath)) Directory.CreateDirectory(Application.StartupPath);

                database = new SQLiteConnection(Path.Combine(Application.StartupPath, databaseName));
            }

            database.CreateTables(CreateFlags.None, tables);

            return database;
        }
    }
}