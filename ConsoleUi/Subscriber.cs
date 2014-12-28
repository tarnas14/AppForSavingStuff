namespace ConsoleUi
{
    public interface Subscriber
    {
        void Execute(UserCommand userCommand);
    }
}