using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public void Register()
        {
            // Register user logic, e.g. save to database...

            // Send email notification
            EmailSender emailSender = new EmailSender();
            emailSender.SendEmail("Welcome to our platform!", Email);
        }
    }
}
/*
the User class is a Data Model (or Entity).
the User class manages user data (username and email), and contains logic
for registering a user. This violates the SRP because the class has more
than one reason to change. It could change due to:

1. Modifications in user data management – for example adding more fields, such as firstName, gender, hobbies.

2. Modifications to the logic of registering a user, for example we may choose to fetch a user from the database by 
  their username rather than their email.


--- To adhere to the SRP we should separate these responsibilities into separate classes
*/