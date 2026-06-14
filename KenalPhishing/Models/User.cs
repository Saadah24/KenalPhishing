using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KenalPhishing.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Category { get; set; } // Child, Adult, Elder
        public string Role { get; set; }     // Member, Admin

        // ADD THESE TWO LINES:
        public bool IsParent { get; set; }
        public string ChildEmail { get; set; }
        public string ProfilePicture { get; set; }
        public int? LinkedChildId { get; set; }
        public string Phone { get; set; }
    }

}