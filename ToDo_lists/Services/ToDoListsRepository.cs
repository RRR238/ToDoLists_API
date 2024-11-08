using Microsoft.EntityFrameworkCore;
using ToDo_lists.DBContexts;
using ToDo_lists.Entities;

namespace ToDo_lists.Services
{
    public class ToDoListsRepository
    {
        public ToDoListsContext _context { get; set; }
        public ToDoListsRepository(ToDoListsContext context)
        {
            _context = context;
        }

        public async Task AddToDoList(ToDoLists todolist)
        {

            await _context.toDoLists.AddAsync(todolist);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToDoListLink(UsersListsLinks link)
        {

            await _context.usersListsLinks.AddAsync(link);
            await _context.SaveChangesAsync();
        }

        public async Task<ToDoLists> GetItemByName(string name, bool getUsers = false)
        {
            ToDoLists toDoList;
            if (getUsers)
            {
                toDoList = await _context.toDoLists.Include(t => t.usersListsLinks)
                    .FirstOrDefaultAsync(u => u.ToDoListName == name);
            }
            else
            {
                toDoList = await _context.toDoLists
                    .FirstOrDefaultAsync(u => u.ToDoListName == name);
            }
            return toDoList;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoveToDoList(ToDoLists toDoList)
        {
            _context.toDoLists.Remove(toDoList);
            await _context.SaveChangesAsync();
        }
    }
}
