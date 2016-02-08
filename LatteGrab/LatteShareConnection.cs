using System;

using RestSharp;

namespace LatteGrab
{
    public delegate void UploadSuccessful(String url);
    public delegate void UploadError(String error);

    class LatteShareConnection
    {
        private static LatteShareConnection instance = new LatteShareConnection();

        public static LatteShareConnection Instance
        {
            get
            {
                return instance;
            }
        }

        private RestClient client = new RestClient("https://grabpaw.com/api/v1");

        private String username = null;
        private String apiKey = null;

        private UploadSuccessful uploadSuccessfulDelegate = null;
        private UploadError uploadErrorDelegate = null;

        private LatteShareConnection()
        {
            if (Properties.Settings.Default.Username != null)
                username = Properties.Settings.Default.Username;

            if (Properties.Settings.Default.APIKey != null)
                apiKey = Properties.Settings.Default.APIKey;
        }

        public bool RequestAPIKey(String username, String password)
        {
            var request = new RestRequest("/key", Method.POST);

            request.AddParameter("username", username);
            request.AddParameter("password", password);

            IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

            if (response.Data.success)
            {
                this.username = username;
                this.apiKey = response.Data.key;

                System.Diagnostics.Debug.WriteLine(username);
                System.Diagnostics.Debug.WriteLine(apiKey);

                Save();

                return true;
            }

            return false;
        }

        public bool CheckAPIKey()
        {
            if (username == "" || apiKey == "")
                return false;

            var request = new RestRequest("/key", Method.GET);

            request.AddParameter("username", username);
            request.AddParameter("apiKey", apiKey);

            IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

            return (response.Data.success);
        }

        public String UploadFile(String path)
        {
            var request = new RestRequest("/upload", Method.POST);

            request.AddParameter("username", username);
            request.AddParameter("apiKey", apiKey);

            request.AddFile("upload", path);

            request.AddHeader("Content-Type", "multipart/form-data");

            IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

            if (response.Data.success)
            {
                if (uploadSuccessfulDelegate != null)
                    uploadSuccessfulDelegate(response.Data.url);

                return response.Data.url;
            }

            if (uploadErrorDelegate != null)
                uploadErrorDelegate(response.Data.error);

            return null;
        }

        private void Save()
        {
            Properties.Settings.Default.Username = username;
            Properties.Settings.Default.APIKey = apiKey;

            Properties.Settings.Default.Save();
        }

        public void LogOff()
        {
            username = null;
            apiKey = null;

            Save();
        }

        public void SetDelegates(UploadSuccessful success, UploadError error)
        {
            uploadSuccessfulDelegate = success;
            uploadErrorDelegate = error;
        }
    }
}
