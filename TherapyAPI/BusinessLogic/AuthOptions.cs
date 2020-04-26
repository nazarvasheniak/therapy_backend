using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic
{
    public class AuthOptions
    {
        public const string ISSUER = "Therapy_AuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:14534/"; // потребитель токена
        const string KEY = "jd645JHkdH348thdsf3ujd4wk";   // ключ для шифрации
        public const int LIFETIME = 260000; // время жизни в минутах
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
