using System.ComponentModel.DataAnnotations;

namespace Mango.Services.AuthAPI.Models.Dto
{
    public class UserDTO
    {
        [Key]
        public string ID { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }
}
