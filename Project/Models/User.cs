using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class User
    {
        public int Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User()
        {
        }

        public User(int id, DateTime dateOfBirth, string firstName, string lastName, string email, string password)
        : this(dateOfBirth, firstName, lastName, email, password)
        {
            Id = id;
        }

        public User(DateTime dateOfBirth, string firstName, string lastName, string email, string password)
        {
            DateOfBirth = dateOfBirth;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string FullName
        {
            get
            {
                return FirstName + ' ' + LastName;
            }
        }

     
    }
}
