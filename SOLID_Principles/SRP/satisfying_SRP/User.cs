using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP.satisfying_SRP
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}

//in this class we can add more fields and methods thats very closely related to the user

// methods that should go inside this user class
/*
Validate(): Checking if the email format is correct or if the username meets length requirements.
GetFullName(): If you have FirstName and LastName fields, a method to concatenate them.
Deactivate(): Changing a Status property from Active to Inactive.
*/

// methods that should not go inside the user class
/*
SaveToDatabase(): This belongs in a Repository or Service class (e.g., UserRepository).
SendEmail(): This belongs in an Email Service.
LogActivity(): This belongs in a Logger.
*/