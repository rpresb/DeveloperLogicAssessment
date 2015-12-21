using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperLogicAssessment.Models
{
    public class User
    {
        public int OID { get; set; }
        [Required]
        public string UserID { get; set; }
        public string PasswordRaw { get; set; }
        public string PasswordHash { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Verified { get; set; }
    }
}
