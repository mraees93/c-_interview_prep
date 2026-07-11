using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
//DIP violation 
namespace SOLID_Principles.practice_snippets.practice_3
{
    public class Fix_2
    {
        //create interface to implement on parent and child class
        public class TextFileStore
        {
            public void Save(string data)
            {
                System.IO.File.WriteAllText("data.txt", data);
            }
        }

        public class UserManager
        {
            private TextFileStore _store;

            public UserManager()
            {
                _store = new TextFileStore(); //VIOLATION of DIP
            }

            public void RegisterUser(string username)
            {
                _store.Save(username);
            }
        }

    }
}