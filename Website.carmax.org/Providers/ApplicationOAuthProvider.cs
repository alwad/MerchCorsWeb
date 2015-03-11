using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Website.carmax.org.Models;
using System.IdentityModel.Services;
using System.Web;

namespace Website.carmax.org.Providers
{

    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "client_credentials". This occurs when a registered client
        ///             application wishes to acquire an "access_token" to interact with protected resources on it's own behalf, rather than on behalf of an authenticated user. 
        ///             If the web application supports the client credentials it may assume the context.ClientId has been validated by the ValidateClientAuthentication call.
        ///             To issue an access token the context.Validated must be called with a new ticket containing the claims about the client application which should be associated
        ///             with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by malicious callers.
        ///             The default behavior is to reject this grant type.
        ///             See also http://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {

            var adfsUser = CheckAccess();

            if (adfsUser != null)
            {
                System.Security.Claims.ClaimsIdentity ci = new System.Security.Claims.ClaimsIdentity("Bearer");

                //add claims from ADFS
                ci.AddClaims(adfsUser.Claims.Select(c => new Claim(c.Type, c.Value)));

                // add a test claim
                ci.AddClaim(new Claim("http://www.carmax.org/canidoit/", "youbetcha"));

                // add an email claim
                ci.AddClaim(new Claim(ClaimTypes.Email, "test@testcom"));

                // add the requested scopes 
                // in real code, we would check for access/check profile/etc.
                ci.AddClaims(context.Scope.Select(c => new Claim("https://www.carmax.org/auth/scope", c)));

                //add client claim
                ci.AddClaim(new Claim("https://www.carmax.org/auth/client", Guid.NewGuid().ToString()));

                //validate the context so the JWT token will be issued
                context.Validated(ci);

            }

            return base.GrantClientCredentials(context);
        }

        /// <summary>
        /// Checks the access.
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CheckAccess()
        {
            var module = FederatedAuthentication.SessionAuthenticationModule;
            if (!module.ContainsSessionTokenCookie(HttpContext.Current.Request.Cookies))
                return null;

            var cookie = module.CookieHandler.Read();
            var sessionCookie = module.ReadSessionTokenFromCookie(cookie);

            if (sessionCookie.ClaimsPrincipal.Identity.IsAuthenticated)
                return sessionCookie.ClaimsPrincipal;

            return null;
        }

        /// <summary>
        /// Called at the final stage of a successful Token endpoint request. An application may implement this call in order to do any final 
        ///             modification of the claims being used to issue access or refresh tokens. This call may also be used in order to add additional 
        ///             response parameters to the Token endpoint's json response body.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id", and that the correct credentials for that client are
        ///             present on the request. If the web application accepts Basic authentication credentials, 
        ///             context.TryGetBasicCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request header. If the web 
        ///             application accepts "client_id" and "client_secret" as form encoded POST parameters, 
        ///             context.TryGetFormCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request body.
        ///             If context.Validated is not called the request will not proceed further.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.Parameters["client_id"] == _publicClientId)
            {
                context.Validated();
            }

            return base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// Creates the properties.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}