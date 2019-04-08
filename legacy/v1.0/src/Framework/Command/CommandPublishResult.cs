using NEventLite.Exception;

namespace NEventLite.Command
{
    public class CommandPublishResult:ICommandPublishResult
    {
        public bool IsSucess { get; }
        public string FailReason { get; }
        public System.Exception ResultException { get; }

        public CommandPublishResult(bool isSucess, string failReason, System.Exception ex)
        {
            this.IsSucess = isSucess;
            FailReason = failReason;
            ResultException = ex;
        }

        public void EnsurePublished()
        {
            if (this.IsSucess == false)
            {
                throw new CommandExecutionFailedException(
                    $"Command failed with message: {this.FailReason} \n\n {this.ResultException?.Message}");
            }
        }
    }
}
