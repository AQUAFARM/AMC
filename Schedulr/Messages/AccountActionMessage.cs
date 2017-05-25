using Schedulr.Models;

namespace Schedulr.Messages
{
    public class AccountActionMessage
    {
        public Account Account { get; private set; }
        public ListAction Action { get; private set; }

        public AccountActionMessage(Account account, ListAction action)
        {
            this.Account = account;
            this.Action = action;
        }
    }
}