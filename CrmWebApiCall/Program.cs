using static System.Console;

namespace CrmWebApiCall
{
    class Program
    {
        static void Maina(string[] args)
        {
            var webApiCallBAL = new WebApiCallBAL(
                "https://aryan123.crm.dynamics.com/api/data",
                "d8a022b2-ff37-4bcc-a8ac-d3a5853922c7",
                "http://ashv.ml/");

            var accessToken = webApiCallBAL.GetToken().Result;

            var retrievedData = webApiCallBAL.GetData(
                accessToken,
                "accounts?$select=name");

            WriteLine(retrievedData.Result);

            WriteLine(webApiCallBAL.UpdateData(
                accessToken, 
                "contacts(BF0A3DA9-048F-E711-81DF-70106FAA3061)",
                "{\"jobtitle\":\"Consultant\"}").Result);

            ReadLine();
        }
    }
}