namespace EcosCLM.Web.EcosLoginIntegration.Interfaces
{
    public interface IEncryptionService
    {
        public string Encrypt(string encryptString, string encryptionKey);
        public string Decrypt(string cipherText, string encryptionKey);
        public string CreateSalt();
        public string EncryptPassword(string password, string salt);
        public string Base64Encode(string plainText);
        public string Base64Decode(string base64EncodedData);
    }
}
