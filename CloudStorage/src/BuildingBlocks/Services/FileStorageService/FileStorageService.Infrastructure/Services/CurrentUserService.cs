using System;
using System.Linq;
using System.Security.Claims;
using FileStorageService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FileStorageService.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return userId != null ? Guid.Parse(userId) : Guid.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
            }
        }
    }
}