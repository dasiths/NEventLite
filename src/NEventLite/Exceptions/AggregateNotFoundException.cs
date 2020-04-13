namespace NEventLite.Exceptions
{
    public class AggregateNotFoundException: System.Exception
    {
        public AggregateNotFoundException(string msg) : base(msg)
        {
        }
    }
}
