using System;
using System.Security.Cryptography;

namespace isci.Daten
{
    public class dtObjekt_ : dtObjekt
    {
        public dtObjekt_(String Identifikation) : base(Identifikation)
        {
            var felder = GetType().GetFields();
            foreach (var f in felder)
            {
                var typ = f.GetType();

                if (typ == typeof(Dateneintrag))
                {
                    var dt = (Dateneintrag)f.GetValue(this);
                    if (!dt.Identifikation.StartsWith(Identifikation)) dt.Identifikation = Identifikation + "." + dt.Identifikation;
                    dt.parentEintrag = Identifikation;
                    Elemente.Add(dt.Identifikation);
                    this.ElementeLaufzeit.Add(dt);
                }
            }
        }
    }

    public class UdpPacketEncryption
    {
        private byte[] key; // Beispiel-Schlüssel, bitte durch einen sicheren Schlüssel ersetzen

        private void holeKey(string file)
        {
            var file_ = System.IO.File.ReadAllBytes(file);
            key = file_;
        }

        private Aes aes;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;
        private byte[] iv;

        public UdpPacketEncryption(string file)
        {
            holeKey(file);
            aes = Aes.Create();
            //aes.Key = key;
            aes.Mode = CipherMode.CFB;

            //aes.KeySize = 128; // Set the key size to 256 bits
            //aes.GenerateKey();
            //System.IO.File.WriteAllBytes("datei", aes.Key);
            //aes.GenerateIV();

            // Initialisierungsvektor (IV) generieren
            aes.GenerateIV();
            iv = aes.IV;

            encryptor = aes.CreateEncryptor();
            decryptor = aes.CreateDecryptor();
        }

        public byte[] Encrypt(byte[] data, int length)
        {
            byte[] encryptedData = encryptor.TransformFinalBlock(data, 0, length);

            // IV und verschlüsselte Daten kombinieren
            byte[] encryptedPacket = new byte[iv.Length + encryptedData.Length];
            Buffer.BlockCopy(iv, 0, encryptedPacket, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, 0, encryptedPacket, iv.Length, encryptedData.Length);

            return encryptedPacket;
        }

        public byte[] Decrypt(byte[] encryptedPacket)
        {
            /* // IV extrahieren
            var iv = new byte[aes.BlockSize / 8];
            Buffer.BlockCopy(encryptedPacket, 0, iv, 0, iv.Length); */

            // Verschlüsselte Daten extrahieren
            byte[] encryptedData = new byte[encryptedPacket.Length - iv.Length];
            Buffer.BlockCopy(encryptedPacket, iv.Length, encryptedData, 0, encryptedData.Length);

            // Entschlüsselung durchführen
            //aes.IV = iv;
            return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}




