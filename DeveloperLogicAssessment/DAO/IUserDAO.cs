using DeveloperLogicAssessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLogicAssessment.DAO
{
    public interface IUserDAO
    {
        User SaveUser(User user);
        User GetUser(string userID);
        IEnumerable<User> GetAll();
    }
}
