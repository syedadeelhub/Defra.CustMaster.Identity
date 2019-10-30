namespace Defra.CustMaster.Identity.CoreApp
{
    public class D365ClientCredential
    {
        public D365ClientCredential(string clientId, string secret)
        {
            ClientId = clientId;
            Secret = secret;
        }

        public string ClientId { get; set; }

        public string Secret { get; set; }

    }
}
