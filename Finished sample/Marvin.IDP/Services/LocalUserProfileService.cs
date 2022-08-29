using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System.Security.Claims;

namespace Marvin.IDP.Services
{
    public class LocalUserProfileService : IProfileService
    {
        private readonly ILocalUserService _localUserService;

        public LocalUserProfileService(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = (await _localUserService
                .GetUserClaimsBySubjectAsync(subjectId))
                .ToList();

            context.AddRequestedClaims(
                claimsForUser.Select(c => new Claim(c.Type, c.Value)).ToList());

        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            //var subjectId = context.Subject.GetSubjectId();
            //context.IsActive = await _localUserService
            //    .IsUserActive(subjectId);
        }
    }
}
