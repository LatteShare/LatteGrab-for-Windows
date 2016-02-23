using System;

using RestSharp;

namespace LatteGrabCore
{
    public delegate void UploadSuccessful(String url);
    public delegate void UploadError(String error);

    public struct LatteShareUserInformation
    {
        public String username;
        public String group;
        public Int64 quota;
        public Int64 usedDiskSpace;
    }

    public class LatteShareConnection
    {
        private static LatteShareConnection instance = new LatteShareConnection();

        public static LatteShareConnection Instance
        {
            get
            {
                return instance;
            }
        }

        private RestClient client = null;

        private static String ServerConnectionAppendString
        {
            get
            {
                return "/api/" + Version;
            }
        }

        private String currentServer = null;

        public String CurrentServer
        {
            get
            {
                if (currentServer == null)
                    return "https://grabpaw.com";

                return currentServer;
            }

            set
            {
                currentServer = value;

                client = new RestClient(currentServer + ServerConnectionAppendString);
            }
        }

        public static String Version
        {
            get
            {
                return "v1";
            }
        }

        private String username = null;
        private String apiKey = null;

        private UploadSuccessful uploadSuccessfulDelegate = null;
        private UploadError uploadErrorDelegate = null;

        private Settings settings = null;

        private LatteShareConnection()
        {   
            try
            {
                settings = Settings.Deserialize(Settings.DefaultLocation());
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine("Can't load settings - creating new..." + e);

                settings = new Settings();
            }

            client = new RestClient(CurrentServer + ServerConnectionAppendString);

            if (settings.Username != null)
                username = settings.Username;

            if (settings.APIKey!= null)
                apiKey = settings.APIKey;

            if (settings.Server != null)
                CurrentServer = settings.Server;
        }

        public bool RequestAPIKey(String username, String password)
        {
            try
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
            catch
            {
                return false;
            }
        }

        public bool CheckAPIKey()
        {
            try
            {
                if (username == "" || apiKey == "")
                    return false;

                var request = new RestRequest("/key", Method.GET);

                request.AddParameter("username", username);
                request.AddParameter("apiKey", apiKey);

                IRestResponse<LatteShareResponse> response = client.Execute<LatteShareResponse>(request);

                return (response.Data.success);
            }
            catch
            {
                return false;
            }
        }

        public LatteShareUserInformation? GetUserInformation()
        {
            try
            {
                var request = new RestRequest("/user", Method.GET);

                request.AddParameter("apiKey", apiKey);

                IRestResponse<LatteShareUserInformationResponse> response = client.Execute<LatteShareUserInformationResponse>(request);

                if (response.Data.success)
                {
                    var userInfo = new LatteShareUserInformation();

                    userInfo.username = (String)response.Data.data["username"];
                    userInfo.group = (String)response.Data.data["group"];
                    userInfo.quota = (long)response.Data.data["quota"];
                    userInfo.usedDiskSpace = (long)response.Data.data["usedDiskSpace"];

                    return userInfo;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public String UploadFile(String path)
        {
            try
            {
                var request = new RestRequest("/upload", Method.POST);

                request.AddParameter("username", username);
                request.AddParameter("apiKey", apiKey);

                request.AddFile("upload", path, System.Web.MimeMapping.GetMimeMapping(path));

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
            catch
            {
                return null;
            }
        }

        private void Save()
        {
            settings.Username = username;
            settings.APIKey = apiKey;
            settings.Server = CurrentServer;

            Settings.Serialize(Settings.DefaultLocation(), settings);
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
