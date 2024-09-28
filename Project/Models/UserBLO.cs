using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class UserBLO
    {
        private UserDAO userDAO = new UserDAO();

        public User Login(string email, string password)
        {
            User user = userDAO.GetUser(email);
            if (user == null)
                throw new Exception("User not found");
            if (!user.Password.Equals(password))
                throw new Exception("Wrong password");
            return user;
        }
        
        public void Register(string firstName, string lastName, DateTime dateOfBirth, 
            string email, string password, string confirmPassword)
        {
            if (!password.Equals(confirmPassword))
                throw new Exception("Password and confirm password do not match");

            if (userDAO.GetUser(email) != null)
                throw new Exception("User already exists");

            User user = new User(dateOfBirth, firstName, lastName, email, password);
            if (!userDAO.InsertUser(user))
                throw new Exception("User could not be inserted");
        }

        public User Update(string firstName, string lastName, DateTime dateOfBirth,
            string password, User loggedUser)
        {
            if (!password.Equals(loggedUser.Password))
                throw new Exception("Wrong password");

            User updatedUser = new User();
            updatedUser.Id = loggedUser.Id;
            updatedUser.FirstName = firstName;
            updatedUser.LastName = lastName;
            updatedUser.DateOfBirth = dateOfBirth;
            updatedUser.Email = loggedUser.Email;
            updatedUser.Password = password;

            if (!userDAO.UpdateUser(updatedUser))
                throw new Exception("Account could not be updated");
            return updatedUser;
        }

        public void Delete(User loggedUser)
        {
            if (!userDAO.DeleteUser(loggedUser))
                throw new Exception("Deletion failed");
        }
    }
}
