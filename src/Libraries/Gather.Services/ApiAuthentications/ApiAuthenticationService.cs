using System;
using System.Collections.Generic;
using System.Linq;
using Gather.Core.Data;
using Gather.Core.Domain.Common;
using Gather.Core.Domain.Api;
using Gather.Services.Security;

namespace Gather.Services.ApiAuthentications
{
    public class ApiAuthenticationService : IApiAuthenticationService
    {

        #region Fields

        private readonly IRepository<ApiAuthentication> _apiAuthenticationRepository;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Constructors

        public ApiAuthenticationService(IRepository<ApiAuthentication> apiAuthenticationRepository, IEncryptionService encryptionService)
        {
            _apiAuthenticationRepository = apiAuthenticationRepository;
            _encryptionService = encryptionService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns if the user has access to the web api
        /// </summary>
        /// <param name="token">The token</param>        
        /// <returns>ApiAuthentication</returns>
        public ApiAuthenticationType ValidateToken(string token)
        {
            try
            {
                string decryptToken = _encryptionService.DecryptText(token);

                var apiAuthentication = GetApiAuthenticationByDecrypted(decryptToken);

                if (apiAuthentication != null)
                {
                    return ApiAuthenticationType.Valid;   
                }
            }
            catch (Exception)
            {
                return ApiAuthenticationType.Invalid;     
            }

            return ApiAuthenticationType.Invalid;            
        }

        /// <summary>
        /// Insert an api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        public void InsertApiUsage(ApiAuthentication api)
        {
            if (api == null)
                throw new ArgumentNullException("api");

            api.LastModifiedBy = api.ApiUser.Id;
            api.CreatedDate = DateTime.Now;
            api.LastModifiedDate = DateTime.Now;
            api.Active = true;

            _apiAuthenticationRepository.Insert(api);

            api.AccessToken = _encryptionService.EncryptText(api.SecretKey + api.WebsiteAddress);
        }

        /// <summary>
        /// Updates an api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        public void UpdateApiUsage(ApiAuthentication api)
        {
            if (api == null)
                throw new ArgumentNullException("api");

            api.LastModifiedDate = DateTime.Now;
            _apiAuthenticationRepository.Update(api);
            api.AccessToken = _encryptionService.EncryptText(api.SecretKey + api.WebsiteAddress);
        }

        /// <summary>
        /// Delete an api usage record
        /// </summary>
        /// <param name="api">ApiAuthentication</param>
        public void DeleteApiUsage(ApiAuthentication api)
        {
            if (api == null)
                throw new ArgumentNullException("api");

            if (api.Deleted)
                return;

            _apiAuthenticationRepository.Delete(api);

        }

        /// <summary>
        /// Get an ApiAuthentication by user & id
        /// </summary>
        /// <param name="userId">User Id</param>        
        /// <param name="id">Id</param>        
        /// <returns>ApiAuthentication</returns>
        public ApiAuthentication GetApiAuthenticationByUserIdandId(int userId, int id)
        {
            if (userId == 0)
                return null;

            if (id == 0)
                return null;

            var query = _apiAuthenticationRepository.Table;

            query = query.Where(u => !u.Deleted);
            query = query.Where(u => u.Active);
            query = query.Where(u => u.ApiUser.Id == userId && u.Id == id);

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get an ApiAuthentication by credentials
        /// </summary>
        /// <param name="token">The token</param>        
        /// <returns>ApiAuthentication</returns>
        private ApiAuthentication GetApiAuthenticationByDecrypted(string token)
        {
            if (token == "")
                return null;

            var query = _apiAuthenticationRepository.Table;

            query = query.Where(u => !u.Deleted);
            query = query.Where(u => u.Active);
            query = query.Where(u => u.SecretKey + u.WebsiteAddress == token);

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get all ApiAuthentication records by user
        /// </summary>
        /// <param name="user">Id of the User</param>        
        /// <returns>List of ApiAuthentications</returns>
        public List<ApiAuthentication> GetApiAuthenticationByUser(int user)
        {
            if (user == 0)
                return null;

            var query = _apiAuthenticationRepository.Table;

            query = query.Where(u => !u.Deleted);
            query = query.Where(u => u.Active);
            query = query.Where(u => u.ApiUser.Id == user);

            return query.ToList();
        }

        #endregion

    }
}
