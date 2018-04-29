using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSdisplayer
{
    namespace Model { 
        public class MessagesObs : IObservable<Message>
        {
            protected List<IObserver<Message>> _observers;

            public MessagesObs()
            {
                _observers = new List<IObserver<Message>>();
            }

            public IDisposable Subscribe(IObserver<Message> observer)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
                return new Unsubscriber(_observers, observer);
            }

            private class Unsubscriber : IDisposable
            {
                private List<IObserver<Message>> _observers;
                private IObserver<Message> _observer;

                public Unsubscriber(List<IObserver<Message>> observers, IObserver<Message> observer)
                {
                    this._observers = observers;
                    this._observer = observer;
                }

                public void Dispose()
                {
                    if (this._observer != null && this._observers.Contains(_observer))
                        this._observers.Remove(_observer);
                }
            }

            public void TrackBuffer(Byte[] buffer)
            {
                TrackMessage(new Message(buffer));
            }

            protected void TrackMessage(Message message)
            {
                foreach (var observer in _observers)
                {
                    if (message == null)
                        observer.OnError(new Exception());
                    else
                        observer.OnNext(message);
                }
            }

            public void EndTransmission()
            {
                foreach (var observer in _observers.ToArray())
                    if (_observers.Contains(observer))
                        observer.OnCompleted();

                _observers.Clear();
            }
        }
    }
}
