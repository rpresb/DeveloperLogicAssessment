using DeveloperLogicAssessment.DAO;
using DeveloperLogicAssessment.Models;
using DeveloperLogicAssessment.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace DeveloperLogicAssessment.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        const int MAX_SECONDS = 30;

        private UserDAO userDAO;

        private UserController()
        {
            userDAO = new UserDAO();
        }

        // GET api/User
        public IEnumerable<User> Get()
        {
            return userDAO.GetAll();
        }

        // POST api/User
        public HttpResponseMessage Post([FromBody]User user)
        {
            if (userDAO.GetUser(user.UserID) != null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Conflict, new { message = "User already exists" }));
            }

            User createdUser = userDAO.SaveUser(generateNewPassword(user.UserID));

            if (createdUser == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, new { created = true });
            return response;
        }

        private User generateNewPassword(string userID)
        {
            User newUser = new User();
            newUser.UserID = userID;
            newUser.PasswordRaw = Membership.GeneratePassword(6, 0);
            newUser.PasswordHash = PasswordHelper.Hash(newUser.PasswordRaw);
            newUser.ExpirationDate = DateTime.Now.AddSeconds(MAX_SECONDS);

            return newUser;
        }

        // POST api/User/Login
        [Route("Login")]
        public HttpResponseMessage Login([FromBody]User user)
        {
            User registeredUser = userDAO.GetUser(user.UserID);

            if (registeredUser == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Conflict, new { message = "User not found" }));
            }

            if (!PasswordHelper.ConfirmPassword(user.PasswordRaw, registeredUser.PasswordHash))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Conflict, new { message = "Invalid user or password" }));
            }

            if (registeredUser.Verified)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Conflict, new { message = "This account is already verified" }));
            }

            if (DateTime.Now.Subtract(registeredUser.ExpirationDate).TotalSeconds >= MAX_SECONDS)
            {
                User newUser = generateNewPassword(registeredUser.UserID);
                newUser.OID = registeredUser.OID;
                userDAO.SaveUser(newUser);

                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.PreconditionFailed, new { message = "The current password has expired, a new password has been created." }));
            }

            registeredUser.Verified = true;
            userDAO.SaveUser(registeredUser);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, registeredUser);
            return response;
        }
    }
}
