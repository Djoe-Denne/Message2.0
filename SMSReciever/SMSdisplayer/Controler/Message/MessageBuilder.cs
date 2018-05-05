using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        bool result = value.EncodeMessage(Encoding.Unicode);
                        if(!result)
                        {
                            return;
                        }

                        messagesQueue.Add(value);
                        if(messagesQueue.Count > 2)
                        {
                            messagesQueue.RemoveAt(0);
                        }
                    }));
                    
                }

                public virtual void Unsubscribe()
                {
                    unsubscriber.Dispose();
                }

                internal void CreateSmiley(Canvas canvas, TextBox text, Model.Message message)
                {
                    canvas.Children.Clear();

                    List<Emoji> emojis = message.EmojiList;

                    foreach(Emoji emoji in emojis)
                    {
                        Rect bBox = text.GetRectFromCharacterIndex(emoji.Index);
                        emoji.SetBoundingBox(bBox);
                        canvas.Children.Add( emoji);
                        
                    }
                    canvas.InvalidateVisual();
                }
            }
        }
    }
}
