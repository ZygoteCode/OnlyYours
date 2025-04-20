using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SimpleCrypto;

public class PasswordManager
{
    private Dictionary<string, string> _credentials;

    public PasswordManager()
    {
        _credentials = new Dictionary<string, string>();
    }

    public bool CredentialsExists(string credentialsName)
    {
        return _credentials.ContainsKey(credentialsName);
    }

    public void AddCredentials(string credentialsName)
    {
        _credentials.Add(credentialsName, "");
    }

    public void DeleteCredentials(string credentialsName)
    {
        _credentials.Remove(credentialsName);
    }

    public void SaveAllCredentials(string password)
    {
        byte[] passwordBytes = Utils.GetPasswordHash(Encoding.UTF8.GetBytes(password));
        string jsonString = JsonConvert.SerializeObject(_credentials, Formatting.Indented);

        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
        byte[] jsonLengthBytes = BitConverter.GetBytes(jsonBytes.Length);
        byte[] jsonHashBytes = Utils.GetKeccakHash(jsonBytes);

        byte[] toBeEncrypted = Utils.Combine(jsonHashBytes, jsonLengthBytes, jsonBytes);
        byte[] encrypted = SimpleAES.Encrypt(toBeEncrypted, passwordBytes);
        byte[] finalHash = Utils.GetKeccakHash(encrypted);

        byte[] toBeCompressed = Utils.Combine(passwordBytes.Skip(16).Take(12).ToArray(), finalHash, encrypted);
        byte[] compressed = Utils.Compress(toBeCompressed);
        byte[] result = Utils.Combine(Utils.GetKeccakHash(compressed), compressed);

        File.WriteAllBytes("credentials.plf", Utils.Combine(new byte[4] { 0xFF, 0xFE, 0x0D, 0x0A }, result));
    }

    public void LoadAllCredentials(string password)
    {
        try
        {
            byte[] passwordBytes = Utils.GetPasswordHash(Encoding.UTF8.GetBytes(password));
            byte[] loadedFile = File.ReadAllBytes("credentials.plf").Skip(4).ToArray();

            byte[] loadedFileHash = loadedFile.Take(64).ToArray();
            loadedFile = loadedFile.Skip(64).ToArray();
            byte[] loadedFileCalculatedHash = Utils.GetKeccakHash(loadedFile);

            if (!Utils.CompareByteArrays(loadedFileHash, loadedFileCalculatedHash))
            {
                MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.CurrentPassword = "";
                return;
            }

            loadedFile = Utils.Decompress(loadedFile);

            byte[] passwordPortion = passwordBytes.Skip(16).Take(12).ToArray();
            byte[] internalPasswordPortion = loadedFile.Take(12).ToArray();
            loadedFile = loadedFile.Skip(12).ToArray();

            if (!Utils.CompareByteArrays(passwordPortion, internalPasswordPortion))
            {
                MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.CurrentPassword = "";
                return;
            }

            byte[] finalHash = loadedFile.Take(64).ToArray();
            loadedFile = loadedFile.Skip(64).ToArray();
            byte[] initialHash = Utils.GetKeccakHash(loadedFile);

            if (!Utils.CompareByteArrays(finalHash, initialHash))
            {
                MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.CurrentPassword = "";
                return;
            }

            byte[] decrypted = SimpleAES.Decrypt(loadedFile, passwordBytes);

            byte[] jsonHashBytes = decrypted.Take(64).ToArray();
            decrypted = decrypted.Skip(64).ToArray();

            byte[] jsonLengthBytes = decrypted.Take(4).ToArray();
            decrypted = decrypted.Skip(4).ToArray();

            int jsonLength = BitConverter.ToInt32(jsonLengthBytes, 0);

            if (decrypted.Length != jsonLength)
            {
                MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.CurrentPassword = "";
                return;
            }

            byte[] calculatedHash = Utils.GetKeccakHash(decrypted);

            if (!Utils.CompareByteArrays(calculatedHash, jsonHashBytes))
            {
                MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.CurrentPassword = "";
                return;
            }

            _credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(decrypted));
        }
        catch
        {
            MessageBox.Show("Invalid password, please try again with another password.", "OnlyYours", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Globals.CurrentPassword = "";
        }
    }

    public Dictionary<string, string> GetCredentials()
    {
        return _credentials;
    }

    public void UpdateCredentials(string credentialsName, string credentialsValue)
    {
        _credentials[credentialsName] = credentialsValue;
    }

    public string GetCredentialsValue(string credentialsName)
    {
        return _credentials[credentialsName];
    }
}