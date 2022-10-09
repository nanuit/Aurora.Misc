using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Aurora.Misc.Updater
{
    #region Exceptions
    /// <summary>
    /// Custom exception for UpdateError Event
    /// </summary>
    public class UpdateErrorException : Exception
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="message">exception message</param>
        public UpdateErrorException(string message)
            : base(message) { }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="message">exception message</param>
        /// <param name="inner">inner exception</param>
        public UpdateErrorException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// custom Exception for UpdateNotFound Event
    /// </summary>
    public class UpdateNotFoundException : Exception
    {
        /// <summary>
        /// Update not found Exception do not need and additional information
        /// </summary>
        public UpdateNotFoundException() { }
    }

    #endregion
    
    /// <summary>
    /// class to facilitate an update mechanism based on a web server
    /// </summary>
    public class Updater
    {
        #region Private Members

        private readonly string m_UpdateUrl;

        #endregion

        #region To Life and Die in Starlight

        /// <summary>
        /// Initialize the calls by setting the download metadata Url
        /// </summary>
        /// <param name="updateUrl">url to the download metadata</param>
        public Updater(string updateUrl)
        {
            m_UpdateUrl = updateUrl;
        }

        #endregion

        #region Events
        /// <summary>
        /// delegate handling UpdateFound Events
        /// </summary>
        public delegate void UpdateFoundEventHandler(Update update);
        /// <summary>
        /// delegate handling UpdateError Events
        /// </summary>
        public delegate void UpdateErrorEventHandler(string errorDescription);
        /// <summary>
        /// delegate handling UpdateNotFound Events
        /// </summary>
        public delegate void NoUpdateFoundEventHandler();

        /// <summary>
        /// Event fired when an update was found
        /// </summary>
        public event UpdateFoundEventHandler? UpdateFound;
        /// <summary>
        /// Event fired when the check for an update has failed
        /// </summary>
        public event UpdateErrorEventHandler? UpdateError;
        /// <summary>
        /// Event fired when there is no update available
        /// </summary>

        public event NoUpdateFoundEventHandler? NoUpdateFound;

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

        /// <summary>
        /// Set a proxy to use for connection to the url
        /// </summary>
        /// <param name="proxyServer">proxy server to use</param>
        /// <param name="port">port of the proxy server</param>
        /// <param name="userName">username if authentication is needed</param>
        /// <param name="password">password if authentication is needed</param>
        /// <param name="useSSL">flag to set ssl for proxy connection</param>
        public void SetProxy(string proxyServer, uint port, string userName = "", string password = "", bool useSSL = false)
        {
            m_ProxyString = $@"http{(useSSL ? "s" : "")}://{proxyServer}:{port:d}";
            m_ProxyUserName = userName;
            m_ProxyPassword = password;
        }

        /// <summary>
        /// Initiate the download of downlaod meta information
        /// </summary>
        /// <param name="productVersion">version to check for an update</param>
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
                                          	
                                          WebRequest request = WebRequest.Create(m_UpdateUrl);

                                          if (!string.IsNullOrEmpty(m_ProxyString))
                                              request.Proxy = CreateProxy();
                                          
                                          // If required by the server, set the credentials.
                                          request.Credentials = CredentialCache.DefaultCredentials;
                                          
                                          using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                                          {
                                          
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
