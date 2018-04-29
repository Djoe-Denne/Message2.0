using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMSdisplayer.Model;

namespace SMSdisplayer.Controler.Message
{
    namespace Controler
    {
        namespace Message
        {
            public class MessageBuilder : IObserver<Model.Message>
            {
                protected IDisposable unsubscriber;
                protected MessagesList messagesQueue;
                
                public MessageBuilder(MessagesList queue)
                {
                    messagesQueue = queue;
                }
                
                public virtual void Subscribe(IObservable<Model.Message> provider)
                {
                    if (provider != null)
                        unsubscriber = provider.Subscribe(this);
                }

                public virtual void OnCompleted()
                {
                    this.Unsubscribe();
                }

                public virtual void OnError(Exception e)
                {
                    
                }

                public virtual void OnNext(Model.Message value)
                {
                    value.EncodeMessage(Encoding.Unicode);
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        messagesQueue.Add(value);
                    }));
                }

                public virtual void Unsubscribe()
                {
                    unsubscriber.Dispose();
                }
                
            }
        }
    }
}
