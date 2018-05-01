using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSdisplayer.Utils.Emoji
{
    class EmojiUtils
    {

        public static string EmojiFile(String character)
        {
            Encoding enc = new UTF32Encoding(true, false);
            byte[] bytes = enc.GetBytes(character);

            if(bytes.Length < 4)
            {
                return "";
            }

            string charHexCode = BitConverter.ToString(bytes).Replace("-", "");

            while (charHexCode.ElementAt(0) == '0')
            {
                charHexCode = charHexCode.Remove(0, 1);
            }

            if (charHexCode == String.Empty)
            {
                return "";
            }

            charHexCode = charHexCode.ToLower();

            String fileName = String.Format(".\\Emoji\\emoji_u{0}.png", charHexCode);

            return File.Exists(fileName) ? fileName : "";
        }
    }
}
