using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Application.Models
{
    public class ApplicationUser : User
    {
        private string _firstName;
        private string _lastName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                UpdateFullName();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                UpdateFullName();
            }
        }

        public string FullName { get; private set; }

        private void UpdateFullName()
        {
            FullName = $"{FirstName} {LastName}".Trim();
        }
    }
}
