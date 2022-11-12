using System;
using Identity.Services.Identities;

namespace Identity.Repositories;

[Serializable]
public class UserEmailIdentity : UserIdentity
{
    public override string Identity { get; set; } = Identities.Email;
    public string Email { get => Subject; set => Subject = value; }
    public string Password { get; set; }
    public string? Name { get; set; }
    public string ValidateCode { get; set; }
    public bool Confirmed { get; set; }
    public int InvalidCodeCount { get; set; }
}