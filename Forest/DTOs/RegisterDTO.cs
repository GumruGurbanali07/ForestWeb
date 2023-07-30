using System.ComponentModel.DataAnnotations;

namespace Forest.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }
    }

}
