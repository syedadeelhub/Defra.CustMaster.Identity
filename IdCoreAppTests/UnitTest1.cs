using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System.Threading;

namespace Defra.CustMaster.Identity.CoreApp.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        // http://localhost:54670/api/v1.0/InitialMatch?B2CObjectId=a2c01be1-e697-11e9-8cf9-bffd509b16c4
        const string URL = "http://localhost:54670/api/v1.0/";//"http://defra-cm-dev-id-app.azurewebsites.net/api/";// "http://localhost:54670/api/";
        const string CONTACTID = "4d67479a-1ffb-e911-a813-000d3a20f8d4";
        string _b2cobjectId = "7b1ad2d0-6618-11e8-8d34-de6a58ac90a8";//"8d0fe141-93d7-11e8-9d9f-69f8b87d910d";
        string _serviceId = "5A90DD44-DD9B-E811-A94F-000D3A3A8543";//"40EE9D1F-CD85-E811-A83D-000D3AB4F967";
                                                                   //{"contactid":"97063ff4-f479-e811-a963-000d3a2bccc5","defra_uniquereference":"CID-0000001015","errorCode":200,"errorMsg":null}
        string _Securityword = "password";
        string _Securityhint = "myHint";

        [TestMethod]
        public void InitialMatchTestWithValidInput()
        {
            _b2cobjectId = "7b1ad2d0-1714-11e8-8d27-de6a20ac17a8";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("InitialMatch?B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json

            };


            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                InitialMatchResponse returnValue = JsonConvert.DeserializeObject<InitialMatchResponse>(response.Content);
                Assert.IsNotNull(returnValue.ServiceUserID);
                Assert.AreEqual(CONTACTID, returnValue.ServiceUserID);
                Assert.AreEqual(200, returnValue.ErrorCode);
            }
        }

        [TestMethod]
        public void AuthzTestWithValidInput()
        {
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("Authz?ServiceID=" + _serviceId + "&B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                AuthzResponse returnValue = JsonConvert.DeserializeObject<AuthzResponse>(response.Content);
                Assert.IsTrue(returnValue.Roles.Count > 0);
            }
        }

        [TestMethod]
        public void InitialMatchTestWithInValidInput()
        {
            _b2cobjectId = "7b1ad2d0-7946-11e8-8d36-851e870eee8";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("InitialMatch?B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                InitialMatchResponse returnValue = JsonConvert.DeserializeObject<InitialMatchResponse>(response.Content);
                Assert.AreEqual(400, returnValue.ErrorCode);
                Assert.AreEqual("B2CObjectid is invalid, is not a valid Guid.", returnValue.ErrorMsg);
            }
        }

        [TestMethod]
        public void InitialMatchTestWhereB2CObjectNotExists()
        {
            _b2cobjectId = "5A90DD44-DD9B-E811-A94F-000D3A3A8543";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("InitialMatch?B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                InitialMatchResponse returnValue = JsonConvert.DeserializeObject<InitialMatchResponse>(response.Content);
                Assert.AreEqual(204, returnValue.ErrorCode);
                Assert.AreEqual("No Content", returnValue.ErrorMsg);
            }
        }

        [TestMethod]
        public void AuthzTestWithValidInputWithNoRolesInTheSystem()
        {
            _b2cobjectId = "7b1ad2d0-7946-11e8-8d36-851e870eee8d";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("Authz?ServiceID=" + _serviceId + "&B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                AuthzResponse returnValue = JsonConvert.DeserializeObject<AuthzResponse>(response.Content);
                Assert.AreEqual(204, returnValue.ErrorCode);
                Assert.AreEqual("No Content", returnValue.ErrorMsg);
            }
        }

        [TestMethod]
        public void AuthzTestWithInValidInput()
        {
            _serviceId = "4366196A-6C29-E911-A968-000D3A28D1A";
            _b2cobjectId = "7b1ad2d0-6618-11e8-8d34-de6a58ac90a8";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("Authz?ServiceID=" + _serviceId + "&B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                AuthzResponse returnValue = JsonConvert.DeserializeObject<AuthzResponse>(response.Content);
                Assert.AreEqual(400, returnValue.ErrorCode);
                Assert.AreEqual("ServiceId is invalid, is not a valid Guid.", returnValue.ErrorMsg);
            }
        }

        [TestMethod]
        public void InitialMatchTestWithSecurityWordSetIsTrue()
        {
            _b2cobjectId = "7b1ad2d0-1714-11e8-8d27-de6a20ac17a8";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("InitialMatch?B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };


            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                InitialMatchResponse returnValue = JsonConvert.DeserializeObject<InitialMatchResponse>(response.Content);
                Assert.IsNotNull(returnValue.ServiceUserID);
                Assert.IsTrue(returnValue.securityWordSet);
                Assert.AreEqual(200, returnValue.ErrorCode);
            }
        }

        [TestMethod]
        public void InitialMatchTestWithMissingSecurityWordSetFalse()
        {
            _b2cobjectId = "333e5d21-54b7-44d8-940b-de6a65ac0310";
            var client = new RestClient(URL);
            // var key = ApplicationAuthenticator.GetS2SAccessTokenForProdMSAAsync();
            var request = new RestRequest("InitialMatch?B2CObjectId=" + _b2cobjectId, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = client.Get(request);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(response.Content))
            {
                InitialMatchResponse returnValue = JsonConvert.DeserializeObject<InitialMatchResponse>(response.Content);
                Assert.IsNotNull(returnValue.ServiceUserID);
                Assert.IsFalse(returnValue.securityWordSet);
                Assert.AreEqual(200, returnValue.ErrorCode);
            }
        }
    }
}