using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Core
{
    public static class Context
    {
        #region Configuration
        public static Configuration _configuration { get; set; }
        static MongoClient client;
        static IMongoDatabase db;

        public static void Initializer(Configuration configuration)
        {
            _configuration = new Configuration();
            _configuration = configuration;
            client = new MongoClient(_configuration.mongoUrl);
            db = client.GetDatabase(_configuration.dbName);
        }
        public static void Dispose()
        {
            client = null;
        }
        #endregion
        #region Collections

        public static void CreateCollection(string collectionName)
        {
            db.CreateCollection(collectionName);
        }
        public static IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            //if(!CollectionExistsAsync(collectionName).Result)
            //    db.CreateCollection(collectionName);

            return db.GetCollection<T>(collectionName);
        }
        public static IMongoCollection<T> GetCollection<T>()
        {
            return GetCollection<T>(typeof(T).Name + "_COLLECTION");
        }
        #endregion
        #region CRUD
        public static T Insert<T>(T document)
        {
            var collection = GetCollection<T>(typeof(T).Name + "_COLLECTION");
            collection.InsertOne(document);
            return document;
        }
        public static List<T> Insert<T>(List<T> documents)
        {
            GetCollection<T>(typeof(T).Name + "_COLLECTION").InsertManyAsync(documents);
            return documents;
        }
        public static List<T> Insert<T>(string collectioName, List<T> documents)
        {
            GetCollection<T>(collectioName).InsertManyAsync(documents);
            return documents;
        }
        public static T Insert<T>(string collectioName, T document)
        {
            GetCollection<T>(collectioName).InsertOne(document);
            return document;
        }
        public static List<BsonDocument> Insert(string collectioName, List<BsonDocument> documents)
        {
            GetCollection<BsonDocument>(collectioName).InsertManyAsync(documents);
            return documents;
        }
        public static BsonDocument Insert(string collectioName, BsonDocument document)
        {
            GetCollection<BsonDocument>(collectioName).InsertOneAsync(document);
            return document;
        }

        public static void Update<T>(MongoDB.Driver.FilterDefinition<T> filter, Dictionary<string, object> properties, bool updateOne, bool IsUpsert)
        {
            var update = Builders<T>.Update.Set(properties.First().Key, properties.First().Value);
            foreach (var property in properties)
            {
                //to implement multiple updates in a decent way, not done yet, must do task.
                update.AddToSet(property.Key, property.Value);
                if (updateOne)
                    GetCollection<T>()
                        .UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = IsUpsert });
                else
                    GetCollection<T>()
                        .UpdateMany(filter, update, new UpdateOptions { IsUpsert = IsUpsert });
            }

        }
        public static void Update<T>(MongoDB.Driver.FilterDefinition<T> filter, Dictionary<string, object> properties)
        {
            Update<T>(filter, properties, false, true);
        }

        public static void Update<T>(MongoDB.Driver.FilterDefinition<T> filter, Dictionary<string, object> properties, bool updateOne)
        {
            Update<T>(filter, properties, false, true);
        }

        public static void Delete<T>(FilterDefinition<T> filter, bool DeleteOne)
        {
            if (DeleteOne)
                GetCollection<T>().DeleteOne(filter);
            else
                GetCollection<T>().DeleteMany(filter);

        }
        public static void Delete<T>(FilterDefinition<T> filter)
        {
            Delete<T>(filter, false);
        }

        public static List<T> GetDocuments<T>(FilterDefinition<T> filter)
        {
            return GetCollection<T>().FindAsync(filter).Result.ToList<T>();
        }

        public static List<T> GetDocuments<T>(string collectionName, FilterDefinition<T> filter)
        {
            return GetCollection<T>(collectionName).FindAsync(filter).Result.ToList<T>();
        }

        public static T GetDocument<T>(FilterDefinition<T> filter)
        {
            return GetCollection<T>().FindAsync(filter).Result.SingleOrDefault<T>();
        }
        public static T GetDocument<T>(string collectionName, FilterDefinition<T> filter)
        {
            return GetCollection<T>(collectionName).FindAsync(filter).Result.SingleOrDefault<T>();
        }
        #endregion
    }
}