namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string encryptString, string encryptionKey);
        string Decrypt(string cipherText, string encryptionKey);
        string CreateSalt();
        string EncryptPassword(string password, string salt);
        string Base64Encode(string plainText);
        string Base64Decode(string base64EncodedData);
    }
}
