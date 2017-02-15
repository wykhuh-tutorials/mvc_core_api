using System.ComponentModel.DataAnnotations;

namespace CodeCamp.Models
{
    public class CredentialModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
