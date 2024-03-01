using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Signee.Domain.Entities.Display;
using Signee.Domain.Entities.Group;
using Signee.Domain.Entities.View;
using Signee.ManagerWeb.Models.Display;
using Signee.ManagerWeb.Models.Group;
using Signee.Services.Areas.Display.Contracts;
using Signee.Services.Areas.Group.Contracts;

namespace Signee.ManagerWeb.Controllers;


[ApiVersion( 1.0 )]
[ApiController]
[Route("api/[controller]" )]
public class GroupController : ControllerBase
{

    private readonly IGroupService _groupService;
    private readonly IDisplayService _displayService;


    public GroupController(IGroupService groupService, IDisplayService displayService)
    {
        _groupService = groupService;
        _displayService = displayService;
    }
    
    
    
    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetAllGroups()
    {
        try
        {
            var groups = await _groupService.GetAllGroups();
        
            var groupDtos = groups.Select(group => new GroupDto
            {
                Id = group.Id!,
                Name = group.Name!,
                Displays = group.Displays.Select(d => new DisplayApi()
                {
                    Id = d.Id,
                    Name = d.Name,
                    Url = d.Url,
                    PairingCode = d.PairingCode,
                    GroupId = d.GroupId
                }).ToList(),      
                Views = group.Views!.Select(v => v.Id).ToList()
            }).ToList();

            // Debug: Print display information for each group to the console
            foreach (var groupDto in groupDtos)
            {
                Console.WriteLine($"Group ID: {groupDto.Id}, Name: {groupDto.Name}");
                Console.WriteLine("Displays:");
                foreach (var display in groupDto.Displays)
                {
                    Console.WriteLine($"Display ID: {display.Id}, Name: {display.Name}");
                }
            }

            return Ok(groupDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetGroupById(string id)
    {
        try
        {
            var group = await _groupService.GetGroupById(id);
            if (group == null)
                return NotFound();

            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    
    [HttpPost("")]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupApi requestCreateGroup)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var group = new Group
            {
                Name = requestCreateGroup.Name,
                Displays = new List<Display>(),
                Views = new List<View>(),
            };

            // Add the group to the database
            await _groupService.AddToGroup(group);

            // Retrieve the ID from the added group
            var groupId = group.Id;

            // Construct a DTO for the created group
            var groupDto = new GroupDto
            {
                Id = groupId,
                Name = requestCreateGroup.Name,
                Displays = new List<DisplayApi>(), // Initialize as empty list
                Views = new List<string>()     // Initialize as empty list
            };

            return Ok(groupDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(string id, [FromBody] UpdateGroupApi requestUpdateGroup)
    {
        try
        {
            var group = await _groupService.GetGroupById(id);
            if (group == null)
                return NotFound();

            group.Name = requestUpdateGroup.Name;
            await _groupService.UpdateGroup(group);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(string id)
    {
        try
        {
            var group = await _groupService.GetGroupById(id);
            if (group == null)
                return NotFound();

            await _groupService.DeleteById(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{groupId}/display/{displayId}")]
    public async Task<IActionResult> AddDisplayToGroup(string groupId, string displayId)
    {
        try
        {
            await _groupService.AddDisplayToGroup(groupId, displayId);
            
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{groupId}/display/{displayId}")]
    public async Task<IActionResult> RemoveDisplayFromGroup(string groupId, string displayId)
    {
        try
        {
            // Logic to remove display from group
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
}
