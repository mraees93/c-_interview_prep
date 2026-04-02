using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOLID_Principles.SRP.violating_SRP.satisfying_SRP
{
    public class UserService
    {
        public void RegisterUser(User user)
        {
            // register user logic ...

            EmailSender emailSender = new EmailSender();
            emailSender.SendEmail("Welcome to our platform!", user.Email);
        }
    }
}