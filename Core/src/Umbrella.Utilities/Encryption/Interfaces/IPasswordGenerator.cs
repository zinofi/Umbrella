namespace Umbrella.Utilities.Encryption.Interfaces
{
    public interface IPasswordGenerator
    {
        string GeneratePassword(int length = 8, int numbers = 1, int upperCaseLetters = 1);
    }
}