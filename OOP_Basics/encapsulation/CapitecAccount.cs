namespace OOP_Basics.encapsulation
{
    public class CapitecAccount
    {
        private int Password { get; set; }
        private double _balance;

        public CapitecAccount(int password, double intialBalance)
        {
            _balance = intialBalance;
        }

        public double GetBalance()
        {
            return _balance;
        }
    }
}