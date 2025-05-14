using FileMetadataService.Application.Commands.AddFileVersion;
using FileMetadataService.Application.Commands.CreateFile;
using FileMetadataService.Application.Commands.DeleteFile;
using FileMetadataService.Application.Commands.RemoveFileShare;
using FileMetadataService.Application.Commands.RestoreFile;
using FileMetadataService.Application.Commands.ShareFile;
using FileMetadataService.Application.Commands.UpdateFile;
using FileMetadataService.Application.Commands.UpdateFileShare;
using FileMetadataService.Application.Queries.GetDeletedFiles;
using FileMetadataService.Application.Queries.GetFileById;
using FileMetadataService.Application.Queries.GetSharedFiles;
using FileMetadataService.Application.Queries.GetUserFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileMetadataService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFiles()
        {
            var result = await _mediator.Send(new GetUserFilesQuery());

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedFiles()
        {
            var result = await _mediator.Send(new GetSharedFilesQuery());

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedFiles()
        {
            var result = await _mediator.Send(new GetDeletedFilesQuery());

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var result = await _mediator.Send(new GetFileByIdQuery { FileId = id });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] CreateFileCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetFileById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(Guid id, [FromBody] UpdateFileCommand command)
        {
            command.FileId = id;
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var result = await _mediator.Send(new DeleteFileCommand { FileId = id });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreFile(Guid id)
        {
            var result = await _mediator.Send(new RestoreFileCommand { FileId = id });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> ShareFile(Guid id, [FromBody] ShareFileCommand command)
        {
            command.FileId = id;
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}/share/{shareId}")]
        public async Task<IActionResult> UpdateFileShare(Guid id, Guid shareId, [FromBody] UpdateFileShareCommand command)
        {
            command.FileId = id;
            command.ShareId = shareId;
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}/share/{shareId}")]
        public async Task<IActionResult> RemoveFileShare(Guid id, Guid shareId)
        {
            var result = await _mediator.Send(new RemoveFileShareCommand { FileId = id, ShareId = shareId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok();
        }

        [HttpPost("{id}/version")]
        public async Task<IActionResult> AddFileVersion(Guid id, [FromBody] AddFileVersionCommand command)
        {
            command.FileId = id;
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }
    }
}