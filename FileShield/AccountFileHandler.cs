using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static FileShield.Constants;

namespace FileShield
{
    public class AccountFileHandler
    {
        private string login;
        private string password;
        public AccountFileHandler(string login, string password) 
        {
            this.login = login;
            this.password = password;
        }

        public bool isAccountExists()
        {
            List<AccountData> accounts = new List<AccountData>();
            accounts = ReadDataFromAccountsFile(accountDataFilePath);
            foreach (AccountData account in accounts)
            {
                if (account.login.TrimEnd('\0') == login)
                    return true;
            }

            return false;
        }

        public bool CheckAuthorizationData()
        {
            List<AccountData> accounts = new List<AccountData>();
            accounts = ReadDataFromAccountsFile(accountDataFilePath);
            foreach (AccountData account in accounts)
            {
                if (account.login.TrimEnd('\0') == login)
                {
                    int hash = Get4ByteHash_First4Bytes(password);
                    if (hash == account.password) 
                        return true;
                    else 
                        return false;
                }
            }
            return false;
        }

        public void CreateNewAccount()
        {
            int hash = Get4ByteHash_First4Bytes(password);
            var data = new AccountData
            {
                login = login,
                password = hash
            };
            WriteDataToAccountsFile(accountDataFilePath, data);
            CreateUnicFolder();
        }

        private void CreateUnicFolder()
        {
            string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, login);
            Directory.CreateDirectory(filePath);
        }

        private void WriteDataToAccountsFile(string filePath, AccountData data)
        {
            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.Append))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                byte[] loginBytes = System.Text.Encoding.ASCII.GetBytes(data.login);
                writer.Write(loginBytes);
                writer.Write(new byte[256 - loginBytes.Length]);
                writer.Write(data.password);
            }
        }

        private List<AccountData> ReadDataFromAccountsFile(string filePath)
        {
            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);
            List<AccountData> dataList = new List<AccountData>();

            AccountData[] accounts;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                accounts = new AccountData[(int)(fs.Length / Marshal.SizeOf(typeof(AccountData)))];

                for (int i = 0; i < accounts.Length; i++)
                {
                    byte[] loginBytes = reader.ReadBytes(256);
                    accounts[i].login = System.Text.Encoding.ASCII.GetString(loginBytes).TrimEnd('0');
                    accounts[i].password = reader.ReadInt32();
                }
            }

            foreach (var account in accounts)
            {
                dataList.Add(account);
            }

            return dataList;
        }

        private int Get4ByteHash_First4Bytes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                if (bytes.Length < 4)
                {
                    return BitConverter.ToInt32(bytes, 0);
                }

                return BitConverter.ToInt32(bytes, 0); 
            }
        }
    }
}