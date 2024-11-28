using System;
using System.Security.Cryptography;
using System.Text;
using SharpHash.Base;


namespace UserService.RequestHelpers;

public class PasswordHelper
{
    // private static readonly HashAlgorithmName HashAlgorithm =  HashAlgorithmName.SHA3_512;

    private static readonly int Iterations = 3500;
    private static readonly int KeySize = 256;
    // public static string HashPassword(string password, out byte[] salt)
    // {
    //     salt = RandomNumberGenerator.GetBytes(PasswordHelper.KeySize);
    //     var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, PasswordHelper.Iterations, PasswordHelper.HashAlgorithm, PasswordHelper.KeySize);
    //     return Convert.ToHexString(hash);
    // }

    // public static bool HashesMatch(string password, string userSalt, string userHash)
    // {
    //     var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), PasswordHelper.SaltConvertHexString2Bytes(userSalt), PasswordHelper.Iterations, PasswordHelper.HashAlgorithm, PasswordHelper.KeySize);
    //     if (Convert.ToHexString(hash) == userHash){
    //         return true;
    //     }
    //     return false;
    // }

    public static string HashPassword(string password, out byte[] salt)
    {
        
        salt = RandomNumberGenerator.GetBytes(16);
        
        
        var hash = HashFactory.Crypto.CreateSHA1();
        
        
        string combinedString = password + Convert.ToHexString(salt);
        
        
        var result = hash.ComputeString(combinedString, Encoding.UTF8);
        
        return result.ToString();
    }

    public static bool HashesMatch(string password, string userSalt, string userHash)
    {
        
        var hash = HashFactory.Crypto.CreateSHA1();
        
        
        string combinedString = password + userSalt;
        
        
        var result = hash.ComputeString(combinedString, Encoding.UTF8);
        
        return result.ToString() == userHash;
    }

    public static byte[] SaltConvertHexString2Bytes(string salt)
    {
        return Convert.FromHexString(salt);
    }
    public static string SaltConvertBytes2HexString(byte [] salt){
        return Convert.ToHexString(salt);
    }
    public static bool PasswordSaltIsValid(DateTime saltDate, IConfiguration config)
    {
        double saltLifetime = Convert.ToDouble(config["PasswordSaltLifetimeInDays"]);
        if (TimeSpan.FromDays(saltLifetime) >  DateTime.UtcNow.Subtract(saltDate))
        {
            return false;
        }
        return true;
    }

}
/*
Usage:
    byte[] salt; // This will be assigned by the method
    string hashedPassword = hasher.HashPassword("MySecurePassword", out salt);
*/