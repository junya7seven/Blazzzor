using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UserProfileImage
    {
        public Guid UserProfileImageId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string ProfileImagePath { get; set; }
    }
}
