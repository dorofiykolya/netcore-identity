using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Identity.Repositories;

[Serializable, BsonKnownTypes(typeof(UserGoogleIdentity), typeof(UserEmailIdentity), typeof(UserGuestIdentity))]
public class UserIdentity
{
    public virtual string Identity { get; set; }
    public string Subject { get; set; }
}