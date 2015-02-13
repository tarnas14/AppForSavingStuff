namespace Modules.MoneyTracking.CommandHandlers
{
    public interface CommandHandler<in TCommand> where TCommand : Command
    {
        void Execute(TCommand command);
    }
}