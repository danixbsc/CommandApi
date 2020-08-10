using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CommandAPI.Models;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context;

        public CommandsController(CommandContext context) => this._context = context;

        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandsItem()
        {
            return _context.CommandItems;
        }

        // GET:     api/command/id
        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandItem(int id)
        {
            var command = _context.CommandItems.Find(id);

            if(command == null)
            {
                return NotFound();
            }

            return command;
        }
    }
}