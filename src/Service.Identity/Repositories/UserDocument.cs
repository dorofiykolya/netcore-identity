using System;
using System.Collections.Generic;
using Common.Mongo;
using Common.Mongo.Attributes;
using MongoDB.Driver;

namespace Identity.Repositories;

[Serializable]
[Collection("Users")]
[Index(typeof(Indexes))]
public class UserDocument : MongoDocument
{
    public string? Name { get; set; }
    public IList<UserIdentity> Identities { get; set; } = new List<UserIdentity>();
    public IList<UserIdentityRole> Roles { get; set; } = new List<UserIdentityRole>();

    public class Indexes : IndexBuilder<UserDocument>
    {
        public Indexes()
        {
            Index(Ascending(new StringFieldDefinition<UserDocument>($"{nameof(Identities)}")));
            Index(Ascending(new StringFieldDefinition<UserDocument>($"{nameof(Identities)}.{nameof(UserIdentity.Subject)}"))).Unique();
        }
    }
}