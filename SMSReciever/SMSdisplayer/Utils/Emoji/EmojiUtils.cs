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
        protected String currentEmojiCode = String.Empty;

        public string EmojiFile(String character)
        {
            Encoding enc = new UTF32Encoding(true, false);
                String fileName = string.Empty;
            if (character != "‍♂️" && character != "‍♀️")
            {
                byte[] bytes = enc.GetBytes(character);

                if (bytes.Length < 4)
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

                fileName = String.Format(".\\Emoji\\emoji_u{0}.png", charHexCode);

                if (!File.Exists(fileName))
                {
                    currentEmojiCode += currentEmojiCode == String.Empty ? charHexCode : "_" + charHexCode;
                }
                else if (currentEmojiCode == String.Empty)
                {
                    currentEmojiCode = "";
                }
            }
            else if (character == "‍♂️")
            {
                fileName = String.Format(".\\Emoji\\emoji_u{0}_200d_2642.png", currentEmojiCode);
                currentEmojiCode = String.Empty;
            }
            else if (character == "‍♀️")
            {
                fileName = String.Format(".\\Emoji\\emoji_u{0}_200d_2640.png", currentEmojiCode);
                currentEmojiCode = String.Empty;
            }
            return File.Exists(fileName) ? fileName: String.Empty;

        }

        public bool IsEmojiInConstruction()
        {
            return currentEmojiCode != String.Empty;
        }

    }
}
