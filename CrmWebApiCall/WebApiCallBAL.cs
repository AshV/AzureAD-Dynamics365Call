using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CrmWebApiCall
{
    public class WebApiCallBAL
    {
        public WebApiCallBAL(string apiUrl, string clientId, string redirectUrl)
        {
            this.apiUrl = apiUrl;
            this.clientId = clientId;
            this.redirectUrl = redirectUrl;
        }

        /// <summary>
        /// Holds the Authentication context based on the Authentication URL
        /// </summary>
        AuthenticationContext authContext;

        /// <summary>
        /// This is the API data url which we will be using to automatically get the
        ///  a) Resource URL - nothing but the CRM url
        ///  b) Authority URL - the Microsoft Azure URL related to our organization on to which we actually authenticate against
        /// </summary>
        private string apiUrl;

        /// <summary>
        /// Client ID or Application ID of the App registration in Azure
        /// </summary>
        private string clientId;

        /// <summary>
        /// The Redirect URL which we defined during the App Registration
        /// </summary>
        private string redirectUrl;

        public async Task<string> GetToken()
        {
            AuthenticationResult authToken;
            try
            {
                // Get the Resource Url & Authority Url using the Api method. This is the best way to get authority URL
                // for any Azure service api.
                AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(apiUrl)).Result;

                string resourceUrl = ap.Resource;
                string authorityUrl = ap.Authority;

                //Generate the Authority context .. For the sake of simplicity for the post, I haven't splitted these
                // in to multiple methods. Ideally, you would want to use some sort of design pattern to generate the context and store
                // till the end of the program.
                authContext = new AuthenticationContext(authorityUrl, false);

                try
                {
                    //Check if we can get the authentication token w/o prompting for credentials.
                    //With this system will try to get the token from the cache if there is any, if it is not there then will throw error
                    authToken = await authContext.AcquireTokenAsync(resourceUrl, clientId, new Uri(redirectUrl), new PlatformParameters(PromptBehavior.Never));
                }
                catch (AdalException e)
                {
                    if (e.ErrorCode == "user_interaction_required")
                    {
                        // We are here means, there is no cached token, So get it from the service.
                        // You should see a prompt for User Id & Password at this place.
                        authToken = await authContext.AcquireTokenAsync(resourceUrl, clientId, new Uri(redirectUrl), new PlatformParameters(PromptBehavior.Always));
                    }
                    else
                    {
                        throw;
                    }
                }

                return authToken.AccessToken;
            }
            catch
            {
                throw;
            }

        }

        public async Task<string> GetData(string token, string oDataQuery)// accounts?$select=name
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes time out period.

                // Pass the Bearer token as part of request headers.
                httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

                var data = await httpClient.GetAsync(apiUrl + "/v8.2/" + oDataQuery);

                if (data.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // If the status code is success... then print the api output.
                    return await data.Content.ReadAsStringAsync();
                }
                else
                {
                    // Failed .. ???
                    return null;
                }
            }
        }

        public async Task<bool> UpdateData(string token, string entityNameId, string dataToUpdate)// contacts(63A0E5B9-88DF-E311-B8E5-6C3BE5A8B200)  // "{\"jobtitle\":\"Consultant\"}"
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes time out period.

                // Pass the Bearer token as part of request headers.
                httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

                var patchMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri(apiUrl + "/v8.2/" + entityNameId),
                    Content = new StringContent(dataToUpdate, Encoding.UTF8, "application/json"),

                    Method = new HttpMethod("PATCH")
                };

                patchMessage.Headers.Add("Accept", "application/json");
                var response = await httpClient.SendAsync(patchMessage);

                return response.IsSuccessStatusCode;
            }
        }
    }
}