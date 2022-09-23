using snippet_manager.Entities;
using snippet_manager.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace snippet_manager.Common
{
    public class DatabaseQueries
    {
        public const string SnippetQuery = @"SELECT Key.Id, SnippetGroup.Id AS SnippetGroupId, SnippetGroup.Description AS SnippetGroup, SnippetCategory.Id AS SnippetCategoryId, 
                SnippetCategory.Description AS SnippetCategory, SnippetLanguage.Id AS SnippetLanguageId, SnippetLanguage.Description AS SnippetLanguage, Key.Description AS Name,
                Snippet.Id AS SnippetId, Snippet.Keyword AS Keywords, Snippet.Import AS Imports, Snippet.Code AS Code, Key.IsPublic FROM Key LEFT JOIN Snippet ON Snippet.KeyId = Key.Id
                JOIN SnippetGroup ON SnippetGroup.Id = Key.GroupId JOIN SnippetCategory ON SnippetCategory.Id = Key.CategoryId JOIN SnippetLanguage ON SnippetLanguage.Id = Key.LanguageId";
    }

    public class DatabaseHandler
    {
        private readonly SQLiteConnection db;

        public DatabaseHandler(string database = "snippet.db")
        {
            db = new(Path.Combine(Application.StartupPath, database));
        }

        public List<SnippetGroup> GetGroups()
        {
            return db.Table<SnippetGroup>().ToList();
        }
        public List<SnippetCategory> GetCategories()
        {
            return db.Table<SnippetCategory>().ToList();
        }
        public List<SnippetLanguage> GetLanguages()
        {
            return db.Table<SnippetLanguage>().ToList();
        }
        public List<Key> GetKeys()
        {
            return db.Table<Key>().ToList();
        }
        public List<Snippet> GetSnippets()
        {
            return db.Table<Snippet>().ToList();
        }
        public List<SnippetQuery> GetKeysSnippets()
        {
            return db.Query<SnippetQuery>(DatabaseQueries.SnippetQuery).ToList();
        }

        public SnippetGroup? GetGroupById(long id)
        {
            return db.Table<SnippetGroup>()?.SingleOrDefault(g => g.Id == id);
        }
        public SnippetGroup? GetGroupByDescription(string description)
        {
            return db.Table<SnippetGroup>()?.SingleOrDefault(g => g.Description?.Trim().ToLower() == description.Trim().ToLower());
        }
        public SnippetCategory? GetCategoryById(long id)
        {
            return db.Table<SnippetCategory>()?.SingleOrDefault(g => g.Id == id);
        }
        public SnippetCategory? GetCategoryByDescription(string description)
        {
            return db.Table<SnippetCategory>()?.SingleOrDefault(g => g.Description?.Trim().ToLower() == description.Trim().ToLower());
        }
        public SnippetLanguage? GetCategoryLanguageById(long id)
        {
            return db.Table<SnippetLanguage>()?.SingleOrDefault(g => g.Id == id);
        }
        public SnippetLanguage? GetLanguageByDescription(string description)
        {
            return db.Table<SnippetLanguage>()?.SingleOrDefault(g => g.Description?.Trim().ToLower() == description.Trim().ToLower());
        }

        public Key? GetKeyById(long id)
        {
            return db.Table<Key>()?.SingleOrDefault(g => g.Id == id);
        }
        public List<Key>? GetKeysById(long groupId, long languageId, long categoryId)
        {
            if (groupId <= 0 && languageId <= 0 && categoryId <= 0)
                return Array.Empty<Key>().ToList();

            return db.Table<Key>()?.Where(g => g.GroupId == groupId && g.LanguageId == languageId && g.CategoryId == categoryId).ToList();
        }
        public Key? GetKeyByDescription(string description)
        {
            return db.Table<Key>()?.SingleOrDefault(g => g.Description?.Trim().ToLower() == description.Trim().ToLower());
        }
        public Snippet? GetSnippetsById(long id)
        {
            return db.Table<Snippet>()?.SingleOrDefault(g => g.Id == id);
        }


        public void DeleteKeyById(long id)
        {
            db.Delete<Key>(id);
        }
        public void DeleteSnippetById(long id)
        {
            db.Delete<Snippet>(id);
        }





        public void CreateTable<T>()
        {
            db.CreateTable<T>();
        }
        public long? AddGroup(string description)
        {
            var group = new SnippetGroup()
            {
                Description = description
            };

            if (!db.Table<SnippetGroup>().Any(x => x.Description?.ToLower() == description.ToLower()))
            {
                db.Insert(group);
                return GetLastInsertId(db);
            }
            else
            {
                db.Update(group);
                return GetGroupByDescription(description)?.Id ?? null;
            }
        }



        private long AddSnippet(ItemValue snippet)
        {
            var keyId = InsertOrUpdateKey(snippet.Key);

            var dbSnippet = new Snippet()
            {
                Id = snippet.Id,
                KeyId = keyId,
                Code = snippet.Code,
                Import = snippet.Import,
                Keyword = snippet.Keyword
            };

            db.Insert(dbSnippet);
            return GetLastInsertId(db);
        }
        private void UpdateSnippet(ItemValue snippet)
        {
            var keyId = InsertOrUpdateKey(snippet.Key);

            var dbSnippet = new Snippet()
            {
                Id = snippet.Id,
                KeyId = keyId,
                Code = snippet.Code,
                Import = snippet.Import,
                Keyword = snippet.Keyword
            };

            db.Update(dbSnippet);
        }
        public long InsertOrUpdateSnippet(ItemValue snippet)
        {
            if (!db.Table<Snippet>().Any(_ => _.Id == snippet.Id))
            {
                return AddSnippet(snippet);
            }
            else
            {
                UpdateSnippet(snippet);
            }

            return snippet.Id;
        }

        public long? AddCategory(string name)
        {
            var category = new SnippetCategory()
            {
                Description = name
            };

            if (!db.Table<SnippetCategory>().Any(x => x.Description?.ToLower() == name.ToLower()))
            {
                db.Insert(category);
                return GetLastInsertId(db);
            }
            else
            {
                db.Update(category);
                return db.Table<SnippetCategory>()?.SingleOrDefault(_ => _.Description?.ToLower() == name.ToLower())?.Id ?? null;
            }
        }
        public long? AddLanguage(string name)
        {
            var language = new SnippetLanguage()
            {
                Description = name
            };

            if (!db.Table<SnippetLanguage>().Any(x => x.Description?.ToLower() == name.ToLower()))
            {
                db.Insert(language);
                return GetLastInsertId(db);
            }
            else
            {
                db.Update(language);
                return db.Table<SnippetLanguage>()?.SingleOrDefault(_ => _.Description?.ToLower() == name.ToLower())?.Id ?? null;
            }
        }

        private long AddKey(ItemKey key)
        {
            var groupId = AddGroup(key.Group);
            var languageId = AddLanguage(key.Language);
            var categoryId = AddCategory(key.Category);

            var dbKey = new Key()
            {
                Id = key.Id,
                GroupId = groupId.HasValue ? groupId.Value : key.GroupId,
                LanguageId = languageId.HasValue ? languageId.Value : key.LanguageId,
                CategoryId = categoryId.HasValue ? categoryId.Value : key.CategoryId,
                Description = key.Description,
                IsPublic = key.IsPublic
            };

            db.Insert(dbKey);
            return GetLastInsertId(db);
        }
        private void UpdateKey(ItemKey key)
        {
            var dbKey = new Key()
            {
                Id = key.Id,
                GroupId = key.GroupId,
                LanguageId = key.LanguageId,
                CategoryId = key.CategoryId,
                Description = key.Description,
                IsPublic = key.IsPublic
            };

            db.Update(dbKey);
        }
        public long InsertOrUpdateKey(ItemKey key)
        {
            if (!db.Table<Key>().Any(_ => _.Id == key.Id))
            {
                return AddKey(key);
            }
            else
            {
                UpdateKey(key);
            }

            return key.Id;
        }

        public static long GetLastInsertId(SQLiteConnection db)
        {
            return SQLite3.LastInsertRowid(db.Handle);
        }


        public void Close()
        {
            db.Close();
            db.Dispose();
        }
    }
}
