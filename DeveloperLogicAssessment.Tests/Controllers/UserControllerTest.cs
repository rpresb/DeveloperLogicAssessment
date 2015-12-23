using DeveloperLogicAssessment.Controllers;
using DeveloperLogicAssessment.DAO;
using DeveloperLogicAssessment.Models;
using DeveloperLogicAssessment.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace DeveloperLogicAssessment.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        private UserController userController;
        private IUserDAO userDAO;

        [TestInitialize]
        public void SetUp()
        {
            PasswordHelper.setSalt("test");

            this.userDAO = Substitute.For<IUserDAO>();
            userController = new UserController(this.userDAO);
            userController.Request = new HttpRequestMessage();
            userController.Configuration = new HttpConfiguration();
        }

        [TestMethod]
        public void ShouldGetAllUsers()
        {
            var users = new List<User>();
            users.Add(new User()
            {
                UserID = "test",
                PasswordRaw = "testPass",
                PasswordHash = "hashPass",
                Verified = false,
                ExpirationDate = System.DateTime.Now.AddMinutes(-1)
            });

            users.Add(new User()
            {
                UserID = "test1",
                PasswordRaw = "test1Pass",
                PasswordHash = "hash1Pass",
                Verified = true,
                ExpirationDate = System.DateTime.Now.AddMinutes(-2)
            });

            userDAO.GetAll().Returns(users);

            var returnUsers = (List<User>)userController.Get();

            Assert.IsNotNull(returnUsers);
            Assert.AreEqual(2, returnUsers.Count);
            Assert.AreEqual("test", returnUsers[0].UserID);
            Assert.AreEqual("test1", returnUsers[1].UserID);
        }

        [TestMethod]
        public void ShouldCreateUser()
        {
            User user = new User()
            {
                UserID = "rpresb"
            };

            User createdUser = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "testPass",
                PasswordHash = "hashPass",
                Verified = false,
                ExpirationDate = System.DateTime.Now.AddSeconds(30)
            };

            userDAO.GetUser(user.UserID).Returns((User)null);
            userDAO.SaveUser(Arg.Any<User>()).Returns(createdUser);

            var response = userController.Post(user);

            Assert.IsNotNull(response);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "User already exists")]
        public void ShouldNotCreateUserThatAlreadyExists()
        {
            User user = new User()
            {
                UserID = "rpresb"
            };

            userDAO.GetUser(user.UserID).Returns(user);

            var response = userController.Post(user);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "Something is not OK in userDAO")]
        public void ShouldNotCreateUserWhenDaoHasProblem()
        {
            User user = new User()
            {
                UserID = "rpresb"
            };

            userDAO.GetUser(user.UserID).Returns((User)null);
            userDAO.SaveUser(Arg.Any<User>()).Returns((User)null);

            var response = userController.Post(user);

            Assert.Fail();
        }

        [TestMethod]
        public void ShouldVerifyUser()
        {
            User user = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq"
            };

            User registeredUser = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq",
                PasswordHash = "z–ëL´…zíÌ\rÅ”÷MV™F%¹\u0003ÓÅ§\u001fZ°•êƒ´R(",
                Verified = false,
                ExpirationDate = DateTime.Now.AddSeconds(30)
            };

            userDAO.GetUser(user.UserID).Returns(registeredUser);

            var response = userController.Login(user);

            Assert.IsNotNull(response);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(true, response.Content.ReadAsStringAsync().Result.Contains("\"Verified\":true"));

            userDAO.Received().SaveUser(Arg.Any<User>());

        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "User does not exists")]
        public void ShouldNotVerifyUserThatDoesNotExists()
        {
            User user = new User()
            {
                UserID = "rpresb"
            };

            userDAO.GetUser(user.UserID).Returns((User)null);

            var response = userController.Login(user);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "User and password are not valid")]
        public void ShouldNotVerifyUserThatInputsWrongPassword()
        {
            User user = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "wrong"
            };

            User registeredUser = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq",
                PasswordHash = "z–ëL´…zíÌ\rÅ”÷MV™F%¹\u0003ÓÅ§\u001fZ°•êƒ´R(",
                Verified = false,
                ExpirationDate = DateTime.Now.AddSeconds(30)
            };

            userDAO.GetUser(user.UserID).Returns(registeredUser);

            var response = userController.Login(user);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "User is verified")]
        public void ShouldNotVerifyUserThatIsVerified()
        {
            User user = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq"
            };

            User registeredUser = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq",
                PasswordHash = "z–ëL´…zíÌ\rÅ”÷MV™F%¹\u0003ÓÅ§\u001fZ°•êƒ´R(",
                Verified = true,
                ExpirationDate = DateTime.Now.AddSeconds(30)
            };

            userDAO.GetUser(user.UserID).Returns(registeredUser);

            var response = userController.Login(user);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException),
            "User password is expired")]
        public void ShouldNotVerifyUserThatPasswordHasExpired()
        {
            User user = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq"
            };

            User registeredUser = new User()
            {
                UserID = "rpresb",
                PasswordRaw = "e-xhNq",
                PasswordHash = "z–ëL´…zíÌ\rÅ”÷MV™F%¹\u0003ÓÅ§\u001fZ°•êƒ´R(",
                Verified = false,
                ExpirationDate = DateTime.Now.AddSeconds(-30)
            };

            userDAO.GetUser(user.UserID).Returns(registeredUser);

            var response = userController.Login(user);

            Assert.Fail();
        }
    }
}
