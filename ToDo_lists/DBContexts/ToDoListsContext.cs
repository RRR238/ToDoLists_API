using System.Collections.Generic;
using ToDo_lists.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Xml;
using ToDo_lists.Models;

namespace ToDo_lists.DBContexts
{
    public class ToDoListsContext:DbContext
    {
      
            public ToDoListsContext(DbContextOptions<ToDoListsContext> options)
            : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<ToDoLists>()
                    .Property(e => e.toDoList)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<List<ToDoItem>>(v, (JsonSerializerOptions)null)
                    );

                modelBuilder.Entity<UsersListsLinks>()
                    .HasOne(link => link.toDoList)
                    .WithMany(list => list.usersListsLinks)
                    .HasForeignKey(link => link.toDoListsId)
                    .OnDelete(DeleteBehavior.Cascade);
             }

            public DbSet<ToDoLists> toDoLists { get; set; }
            public DbSet<UsersListsLinks> usersListsLinks { get; set; }

            public DbSet<Users> users { get; set; }

        
    }
}
