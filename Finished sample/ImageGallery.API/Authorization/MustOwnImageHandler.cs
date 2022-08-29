using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.API.Authorization
{
    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustOwnImageHandler(IGalleryRepository galleryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _galleryRepository = galleryRepository ??
                throw new ArgumentNullException(nameof(galleryRepository));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }



        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            MustOwnImageRequirement requirement)
        {
            var imageId = _httpContextAccessor.HttpContext?
                .GetRouteValue("id")?.ToString();

            if (!Guid.TryParse(imageId, out Guid imageIdAsGuid))
            {
                context.Fail();
                return;
            }

            // get the sub claim
            var ownerId = context.User
                .Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            // if it cannot be found, the handler fails 
            if (ownerId == null)
            {
                context.Fail();
                return;
            }

            if (!await _galleryRepository
                .IsImageOwnerAsync(imageIdAsGuid, ownerId))
            {
                context.Fail();
                return;
            }

            // all checks out
            context.Succeed(requirement);
        }
    }
}
