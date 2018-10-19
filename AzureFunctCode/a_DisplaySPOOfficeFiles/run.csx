#load "csomHelper.csx"
 
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System.Net;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

        
    //Set Variable to capture results
    ClientResult<string> result;

    try {

    // parse query parameter and get request body
    var queryPairs = req.GetQueryNameValuePairs();
    dynamic data = await req.Content.ReadAsAsync<object>();

    string docLibandPath = queryPairs.FirstOrDefault(q => string.Compare(q.Key, "docLibandPath", true) == 0)
        .Value;
        docLibandPath = docLibandPath ?? data?.docLibandPath.ToString();

    string siteUrl = queryPairs.FirstOrDefault(q => string.Compare(q.Key, "siteUrl", true) == 0)
        .Value;
        siteUrl = siteUrl ?? data?.siteUrl.ToString();
    /*
    //Note1:Below was used for testing before incorporating SharePoint
    //Uri itemUri = new Uri (new Uri (siteUrl), docLibandPath);
    //Note1: End

    return itemUri == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a path on the query string or in the request body" + req.RequestUri) 
            : req.CreateResponse(HttpStatusCode.OK, itemUri.AbsoluteUri);
    */
    //Format String for Payload to SharePoint
    Uri itemUri = new Uri (new Uri (siteUrl), docLibandPath);
    string itemUrl = (siteUrl ?? data?.siteUrl).TrimEnd('/') + "/" + (docLibandPath ?? data?.path).TrimStart('/');
    string serverRelativeUrl = itemUrl.Substring(itemUrl.IndexOf('/',9));
    log.Info("ITEM URI: " + itemUri);
    log.Info("ITEM Url: " + serverRelativeUrl);
    log.Info("ServerRelative Path: " + serverRelativeUrl);

        // Get Office Online (WOPI) URL
        using (var ctx = await csomHelper.GetClientContext(siteUrl))
        {
            File f = ctx.Web.GetFileByServerRelativeUrl (serverRelativeUrl);
            result = f.ListItemAllFields.GetWOPIFrameUrl(SPWOPIFrameAction.View);
            //File f = ctx.Web.GetFileByServerRelativeUrl (itemUri.PathAndQuery);
            //result = f.ListItemAllFields.GetWOPIFrameUrl(SPWOPIFrameAction.View);


            ctx.Load(f.ListItemAllFields);
            ctx.ExecuteQuery();

        }

        // Return item URL or error
        return itemUri == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a path on the query string or in the request body" + req.RequestUri) 
            : req.CreateResponse(HttpStatusCode.OK, result.Value);

    }
    catch (Exception ex)
    {
        string message = ex.Message + "\n" + ex.StackTrace;
        return req.CreateResponse(HttpStatusCode.BadRequest, "ERROR FOUND: " + message); 
    }
}
