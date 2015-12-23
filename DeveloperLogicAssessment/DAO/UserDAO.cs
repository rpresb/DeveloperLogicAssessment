using DeveloperLogicAssessment.Models;
using Sqo;
using System.Collections.Generic;
using System.Linq;
using NUnit.Mocks;
using System;

namespace DeveloperLogicAssessment.DAO
{
    public class UserDAO : BaseDAO<User>, IUserDAO
    {
        public User SaveUser(User user)
        {
            this.save(user);

            User createdUser = GetUser(user.UserID);
            return createdUser;
        }

        public User GetUser(string userID)
        {
            var query = from User usr in this.DB()
                        where usr.UserID.Equals(userID)
                        select usr;

            var user = query.FirstOrDefault<User>();
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return (from User usr in DB()
                    orderby usr.ExpirationDate descending
                    select usr).ToList<User>();
        }
    }
}
