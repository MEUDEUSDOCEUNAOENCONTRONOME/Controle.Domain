using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
namespace Domain
{
    public class Configuration
    {
        //new MongoUrl("mongodb://localhost:27017/CONTROLE_OD_DB")
        public MongoUrl mongoUrl { get; set; }
        public string dbName { get; set; }
    }
}
