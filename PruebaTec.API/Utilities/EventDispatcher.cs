namespace PruebaTec.API.Utilities
{
    public class EventDispatcher
    {
        public event EventHandler<string>? Success;
        public event EventHandler<string>? Failed;
        public event EventHandler<Exception>? Error;

        public void RaiseSuccess(string msg) => Success?.Invoke(this, msg);
        public void RaiseFailed(string msg) => Failed?.Invoke(this, msg);
        public void RaiseError(Exception ex) => Error?.Invoke(this, ex);
    }
}
