using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ToDo_lists.Models;

namespace ToDo_lists.Entities
{
    public class ToDoLists
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public List<ToDoItem> toDoList { get; set; } = new List<ToDoItem>();

        public string ToDoListName { get; set; }

        public string CreatedBy { get; set; }
        public ICollection<UsersListsLinks> usersListsLinks { get; set; } = new List<UsersListsLinks>();
    }
}
