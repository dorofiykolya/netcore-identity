# API Identity server
### NET CORE 6

---

**using**
- **mongo** - it stores user and identity
- **redis** - it stores access/refresh token (JWT)
- **swagger** - api can be tested

---

**JWT access token, refresh token**

- **e-mail**
  - sign in
  - sign up
  - confirm by e-mail code
- **google**
  - sign in

--- 

## ENVIRONMET

### Admin
- Admin:Email - admin email
- Admin:Password - admin password
### Jwt
- Jwt:SecurityKey - your security key
- Jwt:Issuer - issue, default identity
- Jwt:Audience - audience, default audience
- Jwt:Lifetime - token lifetime, default 00:00:30
### Mongo
- Mongo:ConnectionString - default mongodb://localhost:27017/
- Mongo:DatabaseName - default identity
### Redis
- Redis:ConnectionString - default redis://localhost:6379
### Email  (SMTP settings)
- Email:From - 
- Email:FromName - 
- Email:SmtpUsername - 
- Email:SmtpPassword - 
- Email:Host - 
- Email:ConfigSet - 
- Email:Port - 
- Email:EnableSsl - 
### Password
- Password:RequiredLength - default 6
- Password:RequiredUniqueChars - default 1
- Password:RequireNonAlphanumeric - default true
- Password:RequireLowercase - default true
- Password:RequireUppercase - default true
- Password:RequireDigit - default true
### Google
- Google:ClientId - client id, like ID.apps.googleusercontent.com
- Google:ClientSecret - client secret
### Version
- Major
- Minor
- Build
