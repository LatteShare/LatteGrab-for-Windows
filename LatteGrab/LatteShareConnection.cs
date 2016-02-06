using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

namespace LatteGrab
{
    class LatteShareConnection
    {
        private RestClient client = new RestClient("https://latte.edr.io/api/v1");

        private String username = null;
        private String apiKey = null;

        public LatteShareConnection()
        {

        }

        public bool RequestAPIKey(String username, String password)
        {
            var request = new RestRequest("/key", Method.POST);

            request.AddParameter("username", username);
            request.AddParameter("password", password);

            IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

            if (response.Data.success)
            {
                apiKey = response.Data.key;

                return true;
            }

            return false;
        }

        public bool CheckAPIKey(String key)
        {
            var request = new RestRequest("/key", Method.GET);

            request.AddParameter("key", apiKey);

            IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

            return (response.Data.success);
        }
    }
}
