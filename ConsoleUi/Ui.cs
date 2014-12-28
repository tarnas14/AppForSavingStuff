namespace ConsoleUi
{
    public class Ui
    {
        private readonly UserCommandFactory _userCommandFactory;
        private readonly SubscriberStore _subscribers;

        public Ui(UserCommandFactory userCommandFactory)
        {
            _userCommandFactory = userCommandFactory;
            _subscribers = new SubscriberStore();
        }

        public void UserInput(string userInput)
        {
            var userCommand = _userCommandFactory.CreateUserCommand(userInput);
            var subscribers = _subscribers.GetSubsFor(userCommand.Name);

            foreach (var sub in subscribers)
            {
                sub.Execute(userCommand);
            }
        }

        public void Subscribe(Subscriber subscriber, string commandName)
        {
            _subscribers.Store(subscriber, commandName);
        }
    }
}