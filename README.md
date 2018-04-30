## Controle.Domain
Framework de otimização de utilizações básicas do MongoDB. Útil, mas não terminado.


## How to use


Configure the database using a class named as Configuration:

====================================================================================================================================
using Domain;
using Domain.Core;
using MongoDB.Driver;

namespace <<YourConfigurationNamespace>>
{
    public class ConfigDB
    {
        public ConfigDB()
        {
            Configuration configuration = new Configuration();
            Connection conn = new Connection();
            configuration.dbName = <<DatabaseName>>;
            configuration.mongoUrl = new MongoUrl(<<YourUrlConfiguration>>);
            Context.Initializer(configuration);
        }
        ~ConfigDB()
        {
            Context.Dispose();
        }
    }
}
====================================================================================================================================


You can start writing your Model's code, the framework use the Mongodriver properties typification. Follow the example:
====================================================================================================================================

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;

namespace <<YourModelsNamespace>>
{
    public class User
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string ID { get; set; }
        public bool deleted { get; set; } = false;
        public bool active { get; set; } = true;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime creationDate { get; set; } = DateTime.Now;
        
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
    }
}

====================================================================================================================================

In the database operations class you will to inherit the Configuration class that you created, in this example, the name of the class
is "ConfigDB". So no worries about connection just inherit the responsible class.
And then you can start to write your database operations code, you can follow this example:

====================================================================================================================================

namespace <<YourDBOperationsNamespace>>
{
    public class UserDB : ConfigDB
    {
        public User InsertUser(User user)
        {
            return Context.Insert<User>(user);
        }

        public User FindUserByID(string ID)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(s => s.ID == ID);
            return Context.GetDocument<User>(filter);
        }

        public User FindUserByLoginAndPassword(string login, string password)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(s => s.login == login && s.password == password && s.active == true);
            return Context.GetDocument<User>(filter);
        }

        public List<User> ListActiveUsers()
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(s => s.active == true);
            return Context.GetDocuments<User>(filter);
        }
        public List<User> ListNotActiveUsers()
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(s => s.active == false);
            return Context.GetDocuments<User>(filter);
        }
        public User UpdateUserByID(string ID, Dictionary<string, object> properties)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(u => u.ID == ID);
            Context.Update<User>(filter, properties);
            return Context.GetDocument<User>(filter);
        }
        public void DeleteUserByID(string ID)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Where(u => u.ID == ID);
            Context.Delete<User>(filter);
        }
    }
}

====================================================================================================================================


Make good usage ;) .
