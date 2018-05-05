using SMSdisplayer.Utils.Emoji;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMSdisplayer
{
    namespace Model
    {
        public class Message
        {
            public enum MessageType { SMS, MMS };

            protected String _messageStr;
            protected String _messageBase;
            protected Byte[] _buffer;
            protected Guid _id;

            private List< Emoji> emojiList;



            public String MessageString
            {
                get
                {
                    return _messageStr;
                }
                set
                {
                    _messageStr = value;
                }
            }

            public String MessageBase
            {
                get
                {
                    return _messageBase;
                }
            }

            public List<Emoji> EmojiList 
            { 
                get
                {
                    return emojiList;
                }
            }


            public Message()
            {
                _messageStr = "";
                _id = Guid.NewGuid();

                emojiList = new List<Emoji>();
            }

            public Message(Byte[] message)
            {
                _buffer = message;
                _messageStr="";
                _id = Guid.NewGuid();

                emojiList = new List<Emoji>();
            }

            protected Message(String message)
            {
                _messageStr = message;
                _id = Guid.NewGuid();

                emojiList = new List<Emoji>();
            }


            public bool EncodeMessage(Encoding encoder)
            {
                int size = GetMessageSize();
                if (size == 0)
                {
                    return false;
                }

                try
                {
                    _messageBase = encoder.GetString(_buffer, 0, size);

                    GenerateDisplayedMessage();
                }catch(Exception)
                {
                    return false;
                }

                return true;
            }

            protected int GetMessageSize()
            {
                for(int i = _buffer.Length; i >= 0; i--)
                {
                    if(_buffer[i-1] != 0)
                    {
                        return i % 2 == 0 ? i : i+1 ;
                    }
                }
                return 0;
            }

            private void GenerateDisplayedMessage()
            {
                _messageStr = "";
                bool isSpaced = false;

                String[] messageParts = Regex.Split(_messageBase, @"\uD83D[\uDC00-\uDFFF]|\uD83C[\uDC00-\uDFFF]|\uFFFD");
                EmojiUtils emojiFactory = new EmojiUtils();
                for (int i = 0; i < messageParts.Length-1; i++)
                {
                    string messagePart = messageParts[i];
                    _messageStr += messagePart;
                    int startIndex = _messageStr.Length - (i * 2);
                    String currentChar = _messageBase.Substring(startIndex < 0? 0:startIndex , 2);
                    string file = emojiFactory.EmojiFile(currentChar);
                    if (!isSpaced)
                    {
                        _messageStr += "    ";
                        isSpaced = true;
                    }
                    if(file == String.Empty && emojiFactory.IsEmojiInConstruction() && messageParts[i + 1].Length >= 3)
                    {
                        String sexChar = messageParts[i + 1].Substring(0, 3);
                        if(sexChar == "‍♂️" || sexChar == "‍♀️")
                        {
                            file = emojiFactory.EmojiFile(sexChar);
                            messageParts[i + 1] = messageParts[i + 1].Remove(0, 3);

                        }
                    }

                    if(file == String.Empty)
                    {
                        continue;
                    }
                    emojiList.Add(new Emoji(currentChar, _messageStr.Length-4, file));
                    isSpaced = false;
                }
                _messageStr += messageParts.Last();
            }
        }
    }
}
