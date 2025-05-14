using FileStorageService.Application.Commands.DeleteFile;
using FileStorageService.Application.Commands.RestoreFile;
using FileStorageService.Application.Commands.UploadFile;
using FileStorageService.Application.Queries.GetDownloadUrl;
using FileStorageService.Application.Queries.GetFileById;
using FileStorageService.Application.Queries.GetFileUrl;
using FileStorageService.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FileStorageService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStoredFileRepository _storedFileRepository;
        private readonly IFileStorageProvider _fileStorageProvider;

        public FilesController(
            IMediator mediator,
            IStoredFileRepository storedFileRepository,
            IFileStorageProvider fileStorageProvider)
        {
            _mediator = mediator;
            _storedFileRepository = storedFileRepository;
            _fileStorageProvider = fileStorageProvider;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFileById(Guid fileId)
        {
            var result = await _mediator.Send(new GetFileByIdQuery { FileId = fileId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{fileId}/url")]
        public async Task<IActionResult> GetFileUrl(Guid fileId)
        {
            var result = await _mediator.Send(new GetFileUrlQuery { FileId = fileId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpGet("{fileId}/download")]
        public async Task<IActionResult> GetDownloadUrl(Guid fileId)
        {
            var result = await _mediator.Send(new GetDownloadUrlQuery { FileId = fileId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid fileId)
        {
            var result = await _mediator.Send(new DeleteFileCommand { FileId = fileId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok();
        }

        [HttpPost("{fileId}/restore")]
        public async Task<IActionResult> RestoreFile(Guid fileId)
        {
            var result = await _mediator.Send(new RestoreFileCommand { FileId = fileId });

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok();
        }

        // Direct file access endpoint (for local storage)
        [HttpGet("{ownerId}/{fileId}/{fileName}")]
        [AllowAnonymous] // This endpoint uses the file path as authentication
        public async Task<IActionResult> GetFile(string ownerId, string fileId, string fileName, [FromQuery] string download = null)
        {
            try
            {
                // Reconstruct the path
                var path = Path.Combine(ownerId, fileId, fileName);

                // Get file stream
                var streamResult = await _fileStorageProvider.GetFileStreamAsync(path);
                if (streamResult.IsFailure)
                {
                    return NotFound(new { error = streamResult.Errors });
                }

                // Determine content type
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                // Set response headers
                if (!string.IsNullOrEmpty(download))
                {
                    // If download parameter is provided, set content disposition to attachment
                    return File(streamResult.Value, contentType, download);
                }
                else
                {
                    // Otherwise, set content disposition to inline
                    return File(streamResult.Value, contentType);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}