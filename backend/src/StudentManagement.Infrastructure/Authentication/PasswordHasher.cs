using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using StudentManagement.Application.Interfaces.Authentication;

namespace StudentManagement.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    #region Constants

    private const int InSaltSize = 16;
    private const int InKeySize = 32;
    private const int InIterations = 100_000;
    private const KeyDerivationPrf ObjPrf = KeyDerivationPrf.HMACSHA256;

    #endregion

    #region Public Methods

    public (string Hash, string Salt) Hash(string stPassword)
    {
        if (string.IsNullOrEmpty(stPassword))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(stPassword));
        }

        byte[] lstSalt = RandomNumberGenerator.GetBytes(InSaltSize);
        byte[] lstDerived = KeyDerivation.Pbkdf2(stPassword, lstSalt, ObjPrf, InIterations, InKeySize);

        return (Convert.ToBase64String(lstDerived), Convert.ToBase64String(lstSalt));
    }

    public bool Verify(string stPassword, string stStoredHash, string stStoredSalt)
    {
        if (string.IsNullOrEmpty(stPassword) || string.IsNullOrEmpty(stStoredHash) || string.IsNullOrEmpty(stStoredSalt))
        {
            return false;
        }

        byte[] lstSalt = Convert.FromBase64String(stStoredSalt);
        byte[] lstExpected = Convert.FromBase64String(stStoredHash);
        byte[] lstActual = KeyDerivation.Pbkdf2(stPassword, lstSalt, ObjPrf, InIterations, lstExpected.Length);

        return CryptographicOperations.FixedTimeEquals(lstActual, lstExpected);
    }

    #endregion
}
