namespace Ui
{
    public interface UserCommandFactory
    {
        UserCommand CreateUserCommand(string userInput);
    }
}