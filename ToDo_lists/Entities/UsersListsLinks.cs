using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDo_lists.Entities
{
    public class UsersListsLinks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [ForeignKey("toDoListsId")]
        [JsonIgnore]
        public ToDoLists toDoList { get; set; }
        public int toDoListsId { get; set; }

        public string username { get; set; }


    }
}
