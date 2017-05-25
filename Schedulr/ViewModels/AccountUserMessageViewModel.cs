using JelleDruyts.Windows;

namespace Schedulr.ViewModels
{
    public class AccountUserMessageViewModel
    {
        public string Message { get; private set; }
        public RelayCommand Command { get; private set; }
        public bool CommandAvailable { get; private set; }

        public AccountUserMessageViewModel(string message)
            : this(message, null)
        {
        }

        public AccountUserMessageViewModel(string message, RelayCommand command)
        {
            this.Message = message;
            this.Command = command;
            this.CommandAvailable = (command != null);
        }
    }
}