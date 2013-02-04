using System.Collections.Generic;
using Gather.Core.Domain.Api;
using Gather.Core.Domain.Common;

namespace Gather.Services.ApiAuthentications
{
    public interface IApiAuthenticationService
    {
        /// <summary>
        /// Validates the access token
        /// </summary>
        ApiAuthenticationType ValidateToken(string token);

        /// <summary>
        /// Get an ApiAuthentication by user
        /// </summary>
        /// <param name="user">The token</param>        
        /// <returns>List of ApiAuthentications</returns>
        List<ApiAuthentication> GetApiAuthenticationByUser(int user);

        /// <summary>
        /// Get an ApiAuthentication by user & id
        /// </summary>
        /// <param name="userId">User Id</param>        
        /// <param name="id">Id</param>        
        /// <returns>ApiAuthentication</returns>
        ApiAuthentication GetApiAuthenticationByUserIdandId(int userId, int id);

        /// <summary>
        /// Insert a api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        void InsertApiUsage(ApiAuthentication api);

        /// <summary>
        /// Updates an api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        void UpdateApiUsage(ApiAuthentication api);

        /// <summary>
        /// Delete an api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        void DeleteApiUsage(ApiAuthentication api);
    }
}
