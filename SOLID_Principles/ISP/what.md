An interface should be very closely linked to the class that uses it (the class should use everything thats in the interface) rather than the code that implements it (a bulky interface where some methods arent used).

e.g

violating the ISP would be having one big bulky interface with properties and methods you are not going to use