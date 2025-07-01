using System;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace GS.Core
{
    public class AESGCM
    {
        private const int NonceSize = 12; // GCM 建議的 Nonce 大小
        private const int TagSize = 16;  // GCM Tag 大小 (128 位元)

        public static string Encrypt(string plaintext, string key, string salt = "")
        {
            // 將 Key 和 Salt 結合後進行 Hash 處理，確保 Key 長度符合要求
            byte[] keyBytes = GenerateKey(key, salt);
            byte[] nonce = GenerateNonce(NonceSize); // 使用 NonceSize 生成 nonce

            // 將明文轉為位元組
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // 加密
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(keyBytes), TagSize * 8, nonce);
            cipher.Init(true, parameters);

            byte[] ciphertext = new byte[cipher.GetOutputSize(plaintextBytes.Length)];
            int len = cipher.ProcessBytes(plaintextBytes, 0, plaintextBytes.Length, ciphertext, 0);
            cipher.DoFinal(ciphertext, len);

            // 將 Nonce 和密文結合後轉為 Base64 字串
            byte[] result = new byte[nonce.Length + ciphertext.Length];
            Array.Copy(nonce, 0, result, 0, nonce.Length);
            Array.Copy(ciphertext, 0, result, nonce.Length, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encryptedText, string key, string salt = "")
        {
            // 將 Key 和 Salt 結合後進行 Hash 處理，確保 Key 長度符合要求
            byte[] keyBytes = GenerateKey(key, salt);

            // 將加密字串轉為位元組
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            // 分離 Nonce 和密文
            byte[] nonce = new byte[NonceSize];
            byte[] ciphertext = new byte[encryptedBytes.Length - NonceSize];
            Array.Copy(encryptedBytes, 0, nonce, 0, NonceSize);
            Array.Copy(encryptedBytes, NonceSize, ciphertext, 0, ciphertext.Length);

            // 解密
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(keyBytes), TagSize * 8, nonce);
            cipher.Init(false, parameters);

            byte[] plaintextBytes = new byte[cipher.GetOutputSize(ciphertext.Length)];
            int len = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, plaintextBytes, 0);
            cipher.DoFinal(plaintextBytes, len);

            return Encoding.UTF8.GetString(plaintextBytes).TrimEnd('\0');
        }

        private static byte[] GenerateKey(string key, string salt)
        {
            // 將 Key 和 Salt 結合後進行 SHA256 Hash，確保 Key 長度為 32 bytes
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key + salt));
            }
        }

        private static byte[] GenerateNonce(int size)
        {
            // 生成指定大小的隨機 Nonce
            var nonce = new byte[size];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(nonce);
            }
            return nonce;
        }

    }
}