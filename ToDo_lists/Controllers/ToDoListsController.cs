using Microsoft.AspNetCore.Mvc;
using ToDo_lists.Models;
using ToDo_lists.Entities;
using ToDo_lists.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ToDo_lists.Controllers
{
    [ApiController]
    public class ToDoListsController:ControllerBase
    {
        ToDoListsRepository _toDoListRepo;
        UsersRepository _usersRepo;

        public ToDoListsController(ToDoListsRepository toDoListRepo, UsersRepository usersRepository)
        {
            _toDoListRepo = toDoListRepo;
            _usersRepo = usersRepository;
        }

        [Authorize]
        [HttpPost("createlist")]
        public async Task<IActionResult> CreateList(ToDoListModel toDoListRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            bool usersExists = await _usersRepo.AreUsernamesAvailableAsync(toDoListRequest.owners);

            if(!usersExists)
                return BadRequest(new { message = "Some of the owners you provided are not registered" });

            ToDoLists existingToDoList = await _toDoListRepo.GetItemByName(toDoListRequest.toDoListName);

            if(existingToDoList != null)
                return Conflict(new { message = "Name for to do list already exists. Choose different name." });

            string creatorUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            ToDoLists newToDoList = new ToDoLists {CreatedBy = creatorUsername, toDoList = toDoListRequest.toDoList, ToDoListName = toDoListRequest.toDoListName};
            await _toDoListRepo.AddToDoList(newToDoList);

            UsersListsLinks link = new UsersListsLinks {toDoListsId = newToDoList.id, username = creatorUsername };
            await _toDoListRepo.AddUserToDoListLink(link);

            foreach(string user in toDoListRequest.owners)
            {
                UsersListsLinks newLink;
                newLink = new UsersListsLinks { toDoListsId = newToDoList.id, username = user };
                await _toDoListRepo.AddUserToDoListLink(newLink);
            }

            return Created(string.Empty, new { message = "To Do List added" });

        }

        [HttpGet("getlist/{listName}/{getUsers}")]
        public async Task<IActionResult> GetList(string listName, bool getUsers)
        {
            ToDoLists toDoList = await _toDoListRepo.GetItemByName(listName, getUsers);

            if(toDoList == null)
                return NotFound();

            return Ok(toDoList);
        }

        [Authorize]
        [HttpPost("additem/{listName}")]
        public async Task<IActionResult> AddItem(string listName, ToDoItem item)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            ToDoLists existingToDoList = await _toDoListRepo.GetItemByName(listName, true);

            if (existingToDoList == null)
                return NotFound();

            string currentUser = User.FindFirst(ClaimTypes.Name)?.Value;

            if (!existingToDoList.usersListsLinks.Any(link => link.username == currentUser))
                return BadRequest();

            existingToDoList.toDoList = existingToDoList.toDoList.Append(item).ToList();
            await _toDoListRepo.SaveChangesAsync();
            return Created(string.Empty, new { message = "Item added" });
        }

        [Authorize]
        [HttpPatch("updateflag/{listName}/{itemNumber}/{newFlag}")]
        public async Task<IActionResult> UpdateFlag(string listName, int itemNumber, string newFlag)
        {
            ToDoLists existingToDoList = await _toDoListRepo.GetItemByName(listName, true);

            if (existingToDoList == null)
                return NotFound();

            string currentUser = User.FindFirst(ClaimTypes.Name)?.Value;

            if (!existingToDoList.usersListsLinks.Any(link => link.username == currentUser))
                return BadRequest();

            if(existingToDoList.toDoList.Count  > itemNumber)
                return BadRequest();

            ToDoItem item = existingToDoList.toDoList[itemNumber - 1];
            item.flag = newFlag;
            existingToDoList.toDoList = existingToDoList.toDoList.ToList();
            await _toDoListRepo.SaveChangesAsync();

            return Ok(new { message = "Flag updated successfully" });
        }

        [Authorize]
        [HttpDelete("deletelist/{listName}")]
        public async Task<IActionResult> DeleteList(string listName)
        {
            ToDoLists existingToDoList = await _toDoListRepo.GetItemByName(listName, true);

            if (existingToDoList == null)
                return NotFound();

            string currentUser = User.FindFirst(ClaimTypes.Name)?.Value;

            if (!existingToDoList.usersListsLinks.Any(link => link.username == currentUser))
                return BadRequest();

            await _toDoListRepo.RemoveToDoList(existingToDoList);

            return NoContent();
        }
    }
}
