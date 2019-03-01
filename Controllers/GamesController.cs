using Boardology.API.Data;
using Boardology.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Boardology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IBoardologyRepository _repo;

        public GamesController(IBoardologyRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            var games = await _repo.GetGames();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _repo.GetGame(id);

            return Ok(game);
        }

        [Authorize]
        [HttpPost("{id}/upvote/{gameId}")]
        public async Task<IActionResult> UpvoteGame(int id, int gameId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var upvote = await _repo.GetUpvote(id, gameId);

            if (upvote != null)
            {
                return BadRequest("You already upvoted this game");
            }

            if (await _repo.GetGame(gameId) == null)
            {
                return NotFound();
            }

            upvote = new Upvote
            {
                UpVoterId = id,
                GameId = gameId
            };

            _repo.Add(upvote);

            await _repo.IncreaseUpvotes(gameId);


            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to upvote game");
        }

        [Authorize]
        [HttpPost("{id}/downvote/{gameId}")]
        public async Task<IActionResult> DownvoteGame(int id, int gameId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var downvote = await _repo.GetDownvote(id, gameId);

            if (downvote != null)
            {
                return BadRequest("You already downvoted this game");
            }

            if (await _repo.GetGame(gameId) == null)
            {
                return NotFound();
            }

            downvote = new Downvote
            {
                DownVoterId = id,
                GameId = gameId
            };

            _repo.Add(downvote);

            await _repo.IncreaseDownvotes(gameId);

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to downvote game");
        }
    }
}
