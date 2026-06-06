using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SitiosPersonal.Services.Helpers
{
    public class EncryptionHelper
    {
        private readonly IConfiguration _configuration;

        public EncryptionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Encriptar(string textoPlano)
        {
            string keyString = _configuration["Encryption:Key"];

            if (string.IsNullOrWhiteSpace(keyString) || keyString.Length != 32)
            {
                throw new Exception("La llave de encriptación debe tener exactamente 32 caracteres.");
            }

            byte[] key = Encoding.UTF8.GetBytes(keyString);

            byte[] nonce = RandomNumberGenerator.GetBytes(12);

            byte[] textoBytes = Encoding.UTF8.GetBytes(textoPlano);

            byte[] cipherText = new byte[textoBytes.Length];

            byte[] tag = new byte[16];

            using var aes = new AesGcm(key, 16);

            aes.Encrypt(
                nonce,
                textoBytes,
                cipherText,
                tag
            );

            return "AESGCM:" +
                   Convert.ToBase64String(nonce) + ":" +
                   Convert.ToBase64String(tag) + ":" +
                   Convert.ToBase64String(cipherText);
        }

        public bool Verificar(string textoPlano, string textoEncriptado)
        {
            if (string.IsNullOrWhiteSpace(textoPlano) ||
                string.IsNullOrWhiteSpace(textoEncriptado))
            {
                return false;
            }

            if (!textoEncriptado.StartsWith("AESGCM:"))
            {
                return false;
            }

            try
            {
                string textoDesencriptado =
                    Desencriptar(textoEncriptado);

                return textoPlano == textoDesencriptado;
            }
            catch
            {
                return false;
            }
        }

        public string Desencriptar(string textoEncriptado)
        {
            string keyString = _configuration["Encryption:Key"];

            if (string.IsNullOrWhiteSpace(keyString) || keyString.Length != 32)
            {
                throw new Exception("La llave de encriptación debe tener exactamente 32 caracteres.");
            }

            byte[] key = Encoding.UTF8.GetBytes(keyString);

            string limpio =
                textoEncriptado.Replace("AESGCM:", "");

            string[] partes =
                limpio.Split(':');

            if (partes.Length != 3)
            {
                throw new Exception("Formato de contraseña encriptada inválido.");
            }

            byte[] nonce =
                Convert.FromBase64String(partes[0]);

            byte[] tag =
                Convert.FromBase64String(partes[1]);

            byte[] cipherText =
                Convert.FromBase64String(partes[2]);

            byte[] textoPlano =
                new byte[cipherText.Length];

            using var aes = new AesGcm(key, 16);

            aes.Decrypt(
                nonce,
                cipherText,
                tag,
                textoPlano
            );

            return Encoding.UTF8.GetString(textoPlano);
        }
    }
}
