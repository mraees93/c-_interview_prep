Encapsulation means that the class/object stores its state privately and you can only access its data with public get and set methods.

This prevents unwanted changes and keeps the data secure.


Real life example: ATM machine

Imagine you are using an ATM to withdraw money:

- You enter your PIN (personal data is hidden inside the system).

- You request cash by pressing buttons (methods to access data).

- The ATM only allows valid transactions and prevents direct access to the cash inside.

Bank account functionality explained:

- The balance variable is private, meaning it cannot be accessed directly from outside the class.

- Users can only modify balance through the Deposit() and Withdraw() methods.

- The GetBalance() method provides controlled access to check the balance.