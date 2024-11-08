using Microsoft.EntityFrameworkCore;
using ToDo_lists.DBContexts;
using ToDo_lists.Entities;

namespace ToDo_lists.Services
{
    public class UsersRepository
    {
        
        public ToDoListsContext _context { get; set; }
        public UsersRepository(ToDoListsContext context) 
        { 
            _context = context;
        }

        public async Task AddUser(Users user)
        {
            
            await _context.users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Users> GetItemByName(string name)
        {
            Users user = await _context.users
                .FirstOrDefaultAsync(u => u.name == name);
            return user;
        }

        public async Task<bool> AreUsernamesAvailableAsync(List<string> usernames)
        {
            var existingUsernames = await _context.users
            .Where(u => usernames.Contains(u.name))
            .Select(u => u.name)
            .ToListAsync();

            return existingUsernames.Count == usernames.Count;
        }

    }
}
