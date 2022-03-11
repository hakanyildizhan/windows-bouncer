namespace WindowsBouncer.Core
{
    public class Handler : IDisposable
    {
        private Handler _nextHandler;

        public Handler SetNext(Handler handler)
        {
            this._nextHandler = handler;
            return handler;
        }

        public virtual IList<SecurityEvent> Handle(IList<SecurityEvent> securityEventList)
        {
            if (this._nextHandler != null)
            {
                return this._nextHandler.Handle(securityEventList);
            }
            else
            {
                return securityEventList;
            }
        }

        public virtual void Dispose()
        {
            if (this._nextHandler != null)
            {
                this._nextHandler.Dispose();
            }
        }
    }

}
