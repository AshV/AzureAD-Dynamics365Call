using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System;

class AzFxCall
{
    static void Main(string[] args)
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls |
                                                                        System.Net.SecurityProtocolType.Tls11 |
                                                                        System.Net.SecurityProtocolType.Tls12 |
                                                                        System.Net.SecurityProtocolType.Ssl3;
        // parse query parameter
        // string name = req.GetQueryNameValuePairs()
        //     .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        //     .Value;

        // https://login.microsoftonline.com/15017f46-17d2-456e-b42f-4724ab6059b7/oauth2/authorize

        string username = "Aryan@Aryan123.onmicrosoft.com";
        string password = "Trial#Env1";
        string apiUrl = "https://aryan123.crm.dynamics.com/api/data/v8.2/";
        string adTenant = "https://login.windows.net/common";
        string adClientAppId = "fdab829e-74dd-471d-9c81-431e1e3e0ada";
        AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(apiUrl)).Result;

        string resourceUrl = ap.Resource;
        string authorityUrl = ap.Authority;

        //Generate the Authority context .. For the sake of simplicity for the post, I haven't splitted these
        // in to multiple methods. Ideally, you would want to use some sort of design pattern to generate the context and store
        // till the end of the program.
     var   authContext = new AuthenticationContext(adTenant, false);


       // AuthenticationContext authContext = new AuthenticationContext(adTenant);

        var userCredential = new UserPasswordCredential(username, password);

        AuthenticationResult authenticationResult = authContext.AcquireTokenAsync(apiUrl, adClientAppId, userCredential).Result;


      //  return req.CreateResponse(HttpStatusCode.OK, authenticationResult.CreateAuthorizationHeader());

        //    return name == null
        //         ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        //         : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
    }
}