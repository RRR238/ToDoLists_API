using System.ComponentModel.DataAnnotations;

namespace ToDo_lists.Models
{
    public class UsersModel
    {
        [Required]
        public string name {  get; set; }
        [Required]
        public string password { get; set; }
    }
}
