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
        public async Task<IActionResult> GetGame(int gameId)
        {
            var game = await _repo.GetGame(gameId);

            return Ok(game);
        }

       
        [HttpGet("{gameId}/comments")]
        public async Task<IActionResult> GetComments(int gameId)
        {

            if (await _repo.GetGame(gameId) == null)
            {
                return NotFound();
            }

            var comments = await _repo.GetComments(gameId);

            return Ok(comments);

        }

        [Authorize]
        [HttpPost("{userId}/comment/{gameId}")]
        public async Task<IActionResult> AddComment(int userId, int gameId, Comment comment)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            if (await _repo.GetGame(gameId) == null)
            {
                return NotFound();
            }

            comment = new Comment
            {
                UserId = userId,
                GameId = gameId,
                Content = comment.Content
            };

            _repo.Add(comment);

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to add comment");

        }


        [Authorize]
        [HttpDelete("{userId}/comment/{commentId}")]
        public async Task<IActionResult> DeleteComment(int userId, int commentId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var comment = await _repo.GetComment(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != userId)
            {
                return Unauthorized();
            }

            _repo.Delete(comment);

            
            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete comment");

        }

        [Authorize]
        [HttpPost("{userId}/upvote/{gameId}")]
        public async Task<IActionResult> UpvoteGame(int userId, int gameId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var upvote = await _repo.GetUpvote(userId, gameId);

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
                UpVoterId = userId,
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
        [HttpPost("{userId}/downvote/{gameId}")]
        public async Task<IActionResult> DownvoteGame(int userId, int gameId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var downvote = await _repo.GetDownvote(userId, gameId);

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
                DownVoterId = userId,
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
