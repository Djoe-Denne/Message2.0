using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSdisplayer
{
    namespace Model
    {
        public class Message
        {
            public enum MessageType { SMS, MMS };

            protected String _messageStr;
            protected Byte[] _buffer;
            protected Guid _id;

            

            public String MessageString
            {
                get
                {
                    return _messageStr;
                }
            }

            public Message(Byte[] message)
            {
                this._buffer = message;
                this._id = Guid.NewGuid();

            }

            protected Message(String message)
            {
                this._messageStr = message;
                this._id = Guid.NewGuid();

            }


            public void EncodeMessage(Encoding encoder)
            {
                int size = GetMessageSize();
                if (size == 0)
                {
                    return;
                }

                _messageStr = encoder.GetString(_buffer,0, size);
                

                var enc = new UTF32Encoding(true, false);
                var bytes = enc.GetBytes("🍆");
                var o = BitConverter.ToString(bytes);

                Console.WriteLine(o);
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
        }
    }
}
