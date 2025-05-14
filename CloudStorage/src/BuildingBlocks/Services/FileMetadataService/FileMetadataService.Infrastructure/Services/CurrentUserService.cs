using System;
using System.Linq;
using System.Security.Claims;
using FileMetadataService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FileMetadataService.Infrastructure.Services
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
                // Try to get user ID from "sub" claim (JWT standard)
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

                // If not found, try with ClaimTypes.NameIdentifier as fallback
                if (string.IsNullOrEmpty(userId))
                {
                    userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                return userId != null ? Guid.Parse(userId) : Guid.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                // Check if user has Admin role using standard ClaimTypes.Role
                var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;

                // If not found, check for role claims directly
                if (!isAdmin)
                {
                    isAdmin = _httpContextAccessor.HttpContext?.User?.Claims
                        .Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin") ?? false;
                }

                return isAdmin;
            }
        }
    }
}