namespace Umbrella.Utilities.Encryption.Abstractions
{
    public interface ISecureStringGenerator
    {
        string Generate(int length = 8, int numbers = 1, int upperCaseLetters = 1);
    }
}