namespace OOP_Basics.encapsulation
{
    public class CapitecAccount
    {
        private int? Password { get; set; }
        private double _balance;

        public CapitecAccount(double intialBalance)
        {
            _balance = intialBalance;
        }

        public void SetPassword(int _password)
        {
            Password = _password;
        }

        public void Withdraw(double amount)
        {
            if(Password == null)
            {
                Console.WriteLine("Your password hasnt been set yet");
                return;
            } 
            else
            {
                if(_balance > 0 && _balance > amount)
                {
                    _balance = _balance - amount;
                    // _balance -= amount;
                } 
                else
                {
                    System.Console.WriteLine("Withdrawal amount must be less than your balance");
                }
            }
        }

        public double GetBalance()
        {
            return _balance;
        }
    }
}