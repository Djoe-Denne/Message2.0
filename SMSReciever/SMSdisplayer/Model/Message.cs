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


            public void EncodeMessage(Encoding encoder)
            {
                int size = GetMessageSize();
                if (size == 0)
                {
                    return;
                }

                _messageBase = encoder.GetString(_buffer,0, size);

                GenerateDisplayedMessage();
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

                String[] messageParts = Regex.Split(_messageBase, @"\uD83D[\uDC00-\uDFFF]|\uD83C[\uDC00-\uDFFF]|\uFFFD");

                for(int i = 0; i < messageParts.Length-1; i++)
                {
                    string messagePart = messageParts[i];
                    _messageStr += messagePart;
                    String currentChar = _messageBase.Substring(_messageStr.Length-(i*2), 2);
                    string file = EmojiUtils.EmojiFile(currentChar);
                    emojiList.Add(new Emoji(currentChar, _messageStr.Length, file));
                    _messageStr += "    ";
                }
                _messageStr += messageParts.Last();
            }
        }
    }
}
