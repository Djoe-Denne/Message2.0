using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSdisplayer.Model
{
    public class MessagesList : ObservableCollection<Message>
    {
        public MessagesList() : base()
        {
        }
    }
}
