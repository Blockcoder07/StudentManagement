namespace StudentManagement.Application.Interfaces.Authentication;

public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string stPassword);

    bool Verify(string stPassword, string stStoredHash, string stStoredSalt);
}
