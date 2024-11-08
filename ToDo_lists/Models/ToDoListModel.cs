namespace ToDo_lists.Models
{
    public class ToDoListModel
    {
        public string toDoListName { get; set; }

        public List<ToDoItem> toDoList { get; set; }

        public List<string> owners { get; set; }
    }

    public class ToDoItem
    {
        public string title { get; set; }

        public string text { get; set; }

        public string deadline { get; set; }

        public string flag { get; set; }
    }
}
