using System;

namespace DeveloperLogicAssessment.Models
{
    public class User
    {
        public int OID { get; set; }
        public string UserID { get; set; }
        public string PasswordRaw { get; set; }
        public string PasswordHash { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Verified { get; set; }
    }
}
