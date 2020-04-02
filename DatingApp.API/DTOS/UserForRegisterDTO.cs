using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOS
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Username {get;set;}

        [Required]
        [StringLength(10,MinimumLength = 4,ErrorMessage="Password must be greater than 5 characters !")]      
        public string Password {get;set; }
    }
    
}