using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System.Security.Cryptography.X509Certificates;

public static class csomHelper {

    private static string ClientId = "obscur-ed-for-demo-732448579ebc";
    private static string Cert = "FabianWilliamsPrivateCert.pfx";                      // Fill in name of your cert file and upload it
    private static string CertPassword = "REDACTED"; // TODO: Explore more secure place for this
    private static string Authority = "https://login.windows.net/fabswilly.onmicrosoft.com/";
    private static string Resource = "https://fabswilly.sharepoint.com/";

    public async static Task<ClientContext> GetClientContext(string siteUrl)
    {
        var authenticationContext = new AuthenticationContext(Authority, false);

        // TODO: Substitute your Azure function name for GetDocUrl2 below:
        var certPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "site\\wwwroot\\GetDocUrl2\\", Cert);
        var cert = new X509Certificate2(System.IO.File.ReadAllBytes(certPath),
            CertPassword,
            X509KeyStorageFlags.Exportable |
            X509KeyStorageFlags.MachineKeySet |
            X509KeyStorageFlags.PersistKeySet);

        var authenticationResult = await authenticationContext.AcquireTokenAsync(Resource, new ClientAssertionCertificate(ClientId, cert));
        var token = authenticationResult.AccessToken;

        var ctx = new ClientContext(siteUrl);
        ctx.ExecutingWebRequest += (s, e) =>
        {
            e.WebRequestExecutor.RequestHeaders["Authorization"] = "Bearer " + authenticationResult.AccessToken;
        };

        return ctx;
    }
}