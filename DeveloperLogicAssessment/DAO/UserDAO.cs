using DeveloperLogicAssessment.Models;
using Sqo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLogicAssessment.DAO
{
    public class UserDAO : BaseDAO<User> 
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
