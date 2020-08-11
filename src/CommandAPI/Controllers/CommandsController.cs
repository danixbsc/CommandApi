using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        [HttpPost]
        public ActionResult<Command> PostCommandItem(Command command) 
        {
            _context.CommandItems.Add(command);
            
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                BadRequest();
            }

            return CreatedAtAction("GetCommandItem", new Command(){Id = command.Id}, command);
        }

        [HttpPut]
        public IActionResult PutCommandItem(int id, Command command)
        {
            if(id != command.Id)
            {
                return BadRequest();
            }
           
            _context.Entry(command).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteCommandItem(int id)
        {
            Command command = _context.CommandItems.Find(id);

            if(command == null)
            {
                return BadRequest();
            } 

            _context.CommandItems.Remove(command);
            _context.SaveChanges();

            return Ok();
        }
    }
}