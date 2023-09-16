using System.ComponentModel.DataAnnotations;

namespace Forest.DTOs
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        
        public string Password { get; set; }

        public bool RememberMe { get;set; }

    }
}
