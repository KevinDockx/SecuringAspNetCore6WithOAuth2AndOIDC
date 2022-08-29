using Marvin.IDP.Entities;

namespace Marvin.IDP.Services
{
    public interface ILocalUserService
    {
        Task<bool> ValidateCredentialsAsync(
             string userName,
             string password);

        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(
            string subject);

        Task<User> GetUserByUserNameAsync(
            string userName);

        Task<User> GetUserBySubjectAsync(
            string subject);

        void AddUser
            (User userToAdd);

        Task<bool> IsUserActive(
            string subject);

        Task<bool> SaveChangesAsync();
    }
}
