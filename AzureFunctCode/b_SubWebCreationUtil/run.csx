#r "Newtonsoft.Json"
#load "..\shared\csomHelper.csx"
 
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System.Net;
using System.Text;
using Newtonsoft.Json;

public class CreateSpoSubWeb
{
    public string scUrl {get; set;}
    public string title {get; set;}
    public string description {get; set;}
    public string language { get; set;}
    public string url {get; set;}
    public string useSamePermissionsAsParentSite { get; set;}
    public string template {get; set;}
}

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    string jsonContent = await req.Content.ReadAsStringAsync();
    var pl = JsonConvert.DeserializeObject<CreateSpoSubWeb>(jsonContent);
    log.Info($"Sub Web Title requested is: {pl.title} and the description is: {pl.description}");

    //ClientResult<string> result;
    try {
        // Get the SharePoint Context
        using (var ctx = await csomHelper.GetClientContext(pl.scUrl))
        {
            WebCollection collWeb = ctx.Web.Webs;

            WebCreationInformation webCreationInfo = new WebCreationInformation();
            webCreationInfo.Title = pl.title;
            webCreationInfo.Description = pl.description;
            webCreationInfo.Language = 1033;
            webCreationInfo.Url = pl.url;
            webCreationInfo.UseSamePermissionsAsParentSite = true;
            webCreationInfo.WebTemplate = pl.template;

            Web oNewWebsite = collWeb.Add(webCreationInfo);
            //result = oNewWebsite;
            ctx.ExecuteQuery();

        }
        // Return item URL or error
        return pl.title == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a path on the query string or in the request body" + req.RequestUri) 
            : req.CreateResponse(HttpStatusCode.OK);

    }
    catch (Exception ex) {
        string message = ex.Message + "\n" + ex.StackTrace;
        return req.CreateResponse(HttpStatusCode.BadRequest, "ERROR FOUND: " + message); 
    }

}
