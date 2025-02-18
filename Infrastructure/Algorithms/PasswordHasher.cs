using Domain.Interfaces.IAlgorithms;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Castle.Components.DictionaryAdapter;

namespace Infrastructure.Algorithms;

public class PasswordHasher(IConfiguration configuration) : IPasswordHasher
{
    public string HashPassword(string password)
    {
        MD5 md5 = MD5.Create();
        
        byte[] b = Encoding.ASCII.GetBytes(password);
        byte[] hash = md5.ComputeHash(b);
        
        StringBuilder sb = new StringBuilder();
        foreach (var a in hash)
        {
            sb.Append(a.ToString("x2"));
        }
        return sb.ToString();
    }

    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        if (hashedPassword != HashPassword(password))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}