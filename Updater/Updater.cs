using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Aurora.Misc.Updater
{
    #region Exceptions
    public class UpdateErrorException : Exception
    {
        public UpdateErrorException() { }

        public UpdateErrorException(string message)
            : base(message) { }

        public UpdateErrorException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class UpdateNotFoundException : Exception
    {
        public UpdateNotFoundException() { }

        public UpdateNotFoundException(string message)
            : base(message) { }

        public UpdateNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }

    #endregion

    #region Delegates

   
    #endregion

    public class Updater
    {
        #region Private Members

        private readonly string m_UpdateUrl;

        #endregion

        #region To Life and Die in Starlight

        public Updater(string updateUrl)
        {
            m_UpdateUrl = updateUrl;
        }

        #endregion

        #region Events
        public delegate void UpdateFoundEventHandler(Update update);
        public delegate void UpdateErrorEventHandler(string errorDescription);
        public delegate void NoUpdateFoundEventHandler();

        public event UpdateFoundEventHandler UpdateFound;
        public event UpdateErrorEventHandler UpdateError;
        public event NoUpdateFoundEventHandler NoUpdateFound;

        private void OnUpdateFound(Update update)
        {
            UpdateFound?.Invoke(update);
        }

        private void OnUpdateError(string error)
        {
            UpdateError?.Invoke(error);
        }

        private void OnNoUpdateFound()
        {
            NoUpdateFound?.Invoke();
        }

        #endregion

        #region Properties

        private string m_ProxyString = string.Empty;
        private string m_ProxyUserName = string.Empty;
        private string m_ProxyPassword = string.Empty;

        #endregion

        #region Public Methods

        public void SetProxy(string proxyServer, uint port, string userName = "", string password = "", bool useSSL = false)
        {
            m_ProxyString = $@"http{(useSSL ? "s" : "")}://{proxyServer}:{port:d}";
            m_ProxyUserName = userName;
            m_ProxyPassword = password;
        }

        public void CheckforUpate(Version productVersion)
        {
            Task.Factory.StartNew(() => GetUpdateDataFromUrl(productVersion));
        }

        #endregion

        #region Private Methods

        private WebProxy CreateProxy()
        {
            WebProxy webproxy = new WebProxy {Address = new Uri(m_ProxyString)};
            if (!string.IsNullOrEmpty(m_ProxyUserName))
            {
                webproxy.Credentials = new NetworkCredential(m_ProxyUserName, m_ProxyPassword);
            }
            return (webproxy);
        }

        private void GetUpdateDataFromUrl(Version productVersion)
        {
            Task.Factory.StartNew(() =>
                                  {
                                      try
                                      {
                                          // Create a request for the URL. 		
                                          WebRequest request = WebRequest.Create(m_UpdateUrl);

                                          if (!string.IsNullOrEmpty(m_ProxyString))
                                          {
                                              request.Proxy = CreateProxy();
                                          }
                                          // If required by the server, set the credentials.
                                          request.Credentials = CredentialCache.DefaultCredentials;
                                          // Get the response.
                                          using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                                          {
                                              // Display the status.
                                              // Get the stream containing content returned by the server.
                                              using (Stream dataStream = response.GetResponseStream())
                                              {
                                                  if (dataStream != null)
                                                  {
                                                      using (StreamReader reader = new StreamReader(dataStream))
                                                      {
                                                          OnUpdateFound(new Update(reader, productVersion));
                                                      }
                                                  }
                                              }
                                          }
                                      }
                                      catch (UpdateErrorException exError)
                                      {
                                          OnUpdateError(exError.Message);
                                      }
                                      catch (UpdateNotFoundException)
                                      {
                                          OnNoUpdateFound();
                                      }
                                      catch (Exception ex)
                                      {
                                          OnUpdateError(ex.Message);
                                      }
                                  });
        }

        #endregion
    }
}
