using Microsoft.AspNetCore.Mvc;
using ToDo_lists.Models;
using ToDo_lists.Entities;
using ToDo_lists.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;

namespace ToDo_lists.Controllers
{
    [ApiController]
    public class ToDoListsController:ControllerBase
    {
        private ToDoListsRepository _toDoListRepo;
        private UsersRepository _usersRepo;
       private CallEndpoint _callEndpoint;

        public ToDoListsController(ToDoListsRepository toDoListRepo, UsersRepository usersRepository, 
                                    CallEndpoint callEndpoint)
        {
            _toDoListRepo = toDoListRepo;
            _usersRepo = usersRepository;
            _callEndpoint = callEndpoint;

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

            toDoListRequest.owners.Add(creatorUsername);
            var payload = new
            {
                to_do_list_name = toDoListRequest.toDoListName,
                to_do_list = toDoListRequest.toDoList,
                owners = toDoListRequest.owners,
            };

            var response = await _callEndpoint.PostRequest($"http://{AiServiceConfig.host}:{AiServiceConfig.port}/insert_items",payload);

            return Created(string.Empty, new { message = "To Do List added" });

        }

        [Authorize]
        [HttpPost("similaritysearch")]
        public async Task<IActionResult> GetSimilarItems(SimilaritySearchModel similarityRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            string creatorUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            var payload = new
            {
                limit = similarityRequest.limit,
                text = similarityRequest.text,
                owner = creatorUsername
            };

            var response = await _callEndpoint.PostRequest($"http://{AiServiceConfig.host}:{AiServiceConfig.port}/search_items", payload);
            if (response.IsSuccessStatusCode)
            {
                // Read the JSON body as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResponse);
                SimilaritySearchResponseModel deserializedResponse = JsonSerializer.Deserialize<SimilaritySearchResponseModel>(jsonResponse);
                ToDoLists foundList;
                ToDoItem foundItem;
                List<FoundItemsModel> itemsToReturn = new List<FoundItemsModel>();

                foreach (var item in deserializedResponse.responseItems)
                {
                    foundList = await _toDoListRepo.GetItemByName(item.to_do_list_name);
                    foundItem = foundList.toDoList[item.item_number - 1];
                    itemsToReturn.Add(new FoundItemsModel { Name = item.to_do_list_name, item = foundItem });
                }

                return Ok(itemsToReturn);

            }
            else
            {
                return StatusCode(500, "An error occurred while processing your search.");
            }
            
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
            List<string >owners = existingToDoList.usersListsLinks.Select(link => link.username).ToList();
            var payload = new {to_do_list_name=listName,
                                owners = owners,text=item.text, 
                                item_number= existingToDoList.toDoList.Count};
            var response = await _callEndpoint.PostRequest($"http://{AiServiceConfig.host}:{AiServiceConfig.port}/add_item", payload);

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

            var response = await _callEndpoint.DeleteRequest($"http://{AiServiceConfig.host}:{AiServiceConfig.port}/delete_items/{listName}");

            return NoContent();
        }
    }
}
