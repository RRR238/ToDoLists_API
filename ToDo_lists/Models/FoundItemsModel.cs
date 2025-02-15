namespace ToDo_lists.Models
{
    public class FoundItemsModel
    {
        public string toDoListName { get; set; }

        public int itemNumber { get; set; }
        public ToDoItem item { get; set; }
    }
}