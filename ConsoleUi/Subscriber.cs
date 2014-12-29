namespace Ui
{
    public interface Subscriber
    {
        void Execute(UserCommand userCommand);
    }
}