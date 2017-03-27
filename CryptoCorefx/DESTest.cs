using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCorefx
{
    public class DESTest
    {
        public const string PUBLICKEY = "TEST0327";

        DesEngine engine = new DesEngine();

        public DESTest() { }

        public string Encode(string str)
        {
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
            var key = Encoding.ASCII.GetBytes(PUBLICKEY);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key),key));
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            var outblock = new byte[cipher.GetOutputSize(inputByteArray.Length)];
            var tam = cipher.ProcessBytes(inputByteArray, 0, inputByteArray.Length, outblock, 0);
            try
            {
                cipher.DoFinal(outblock, tam);
            }
            catch (Exception ex)
            {

            }

            StringBuilder ret = new StringBuilder();
            foreach (byte b in outblock)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        public string Decode(string str)
        {
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
            var key = Encoding.ASCII.GetBytes(PUBLICKEY);
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), key));

            byte[] inputByteArray = new byte[str.Length / 2];
            for (int x = 0; x < str.Length / 2; x++)
            {
                int i = (Convert.ToInt32(str.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            var outblock = new byte[cipher.GetOutputSize(inputByteArray.Length)];
            int tam = cipher.ProcessBytes(inputByteArray, 0, inputByteArray.Length, outblock, 0);

            try
            {
                cipher.DoFinal(outblock, tam);
            }
            catch(Exception ex)
            {

            }
            return Encoding.UTF8.GetString(outblock);
        }
    }
}
