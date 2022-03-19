using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace TheWorkBook.Identity
{
    public class ReturnUrlParser : IdentityServer4.Services.IReturnUrlParser
    {
        private readonly IAuthorizeRequestValidator _validator;
        private readonly IUserSession _userSession;
        private readonly ILogger _logger;
        private readonly IAuthorizationParametersMessageStore _authorizationParametersMessageStore;

        public ReturnUrlParser(
            IAuthorizeRequestValidator validator,
            IUserSession userSession,
            ILogger<ReturnUrlParser> logger,
            IAuthorizationParametersMessageStore authorizationParametersMessageStore = null)
        {
            _validator = validator;
            _userSession = userSession;
            _logger = logger;
            _authorizationParametersMessageStore = authorizationParametersMessageStore;
        }


        public bool IsValidReturnUrl(string returnUrl)
        {
            return true;
        }

        public async Task<AuthorizationRequest> ParseAsync(string returnUrl)
        {
            if (IsValidReturnUrl(returnUrl))
            {
                var parameters = returnUrl.ReadQueryStringAsNameValueCollection();
                if (_authorizationParametersMessageStore != null)
                {
                    var messageStoreId = parameters["authzId"];
                    var entry = await _authorizationParametersMessageStore.ReadAsync(messageStoreId);
                    parameters = entry?.Data.FromFullDictionary() ?? new NameValueCollection();
                }

                var user = await _userSession.GetUserAsync();
                var result = await _validator.ValidateAsync(parameters, user);
                if (!result.IsError)
                {
                    _logger.LogTrace("AuthorizationRequest being returned");

                    ValidatedAuthorizeRequest request = result.ValidatedRequest;
                    AuthorizationRequest authorizationRequest = new AuthorizationRequest();

                    authorizationRequest.Client = request.Client;
                    authorizationRequest.RedirectUri = request.RedirectUri;
                    authorizationRequest.DisplayMode = request.DisplayMode;
                    authorizationRequest.UiLocales = request.UiLocales;
                    authorizationRequest.IdP = request.GetIdP();
                    authorizationRequest.Tenant = request.GetTenant();
                    authorizationRequest.LoginHint = request.LoginHint;
                    authorizationRequest.PromptModes = request.PromptModes;
                    authorizationRequest.AcrValues = request.GetAcrValues();
                    authorizationRequest.ValidatedResources = request.ValidatedResources;
                    //authorizationRequest.Parameters = request.Raw;
                    //authorizationRequest.RequestObjectValues = request.RequestObjectValues;

                    //return new AuthorizationRequest(result.ValidatedRequest);

                    return authorizationRequest;
                }
            }

            _logger.LogTrace("No AuthorizationRequest being returned");
            return null;
        }
    }
}
