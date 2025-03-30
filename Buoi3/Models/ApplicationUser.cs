using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Buoi3.Models
{
    public class ApplicationUser : IdentityUser
    {        
        public string Name { get; set; }
        public string? Address { get; set; }
    }

}
