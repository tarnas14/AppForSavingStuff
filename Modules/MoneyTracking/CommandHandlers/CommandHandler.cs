namespace Modules.MoneyTracking.CommandHandlers
{
    internal interface CommandHandler<in TCommand> where TCommand : Command
    {
        void Execute(TCommand command);
    }
}