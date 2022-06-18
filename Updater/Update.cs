using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Aurora.Misc.Process;

namespace Aurora.Misc.Updater
{
    public class Update
    {
        #region Life and Die in Starlight

        public Update(TextReader streamWithUpdateInfo, Version currentVersion)
        {
            if (streamWithUpdateInfo == null)
                throw (new UpdateErrorException("Invalid data stream for update link"));

            // Read the content.
            // First line is Version number
            string helper = streamWithUpdateInfo.ReadLine();
            if (string.IsNullOrEmpty(helper))
            {
                throw (new UpdateErrorException("Invalid control file format: no Version found"));
            }
            VersionNumber = new Version(helper);

            //Second line is download file
            helper = streamWithUpdateInfo.ReadLine();
            if (string.IsNullOrEmpty(helper))
            {
                throw (new UpdateErrorException("Invalid control file format: no file to download"));
            }
            DownloadPath = new Uri(helper);
            //third line is the install command
            Command = streamWithUpdateInfo.ReadLine();
            if (string.IsNullOrEmpty(Command))
                Command = string.Empty;
            Parameters = streamWithUpdateInfo.ReadLine();
            if (string.IsNullOrEmpty(Parameters))
                Parameters = string.Empty;
            if (VersionNumber <= currentVersion)
                throw (new UpdateNotFoundException());

        }

        #endregion

        #region Properties

        /// <summary>
        ///     Version number of the package ready to be downloaded
        /// </summary>
        public Version VersionNumber { get; set; }

        /// <summary>
        ///     Url to the file to be downloaded
        /// </summary>
        public Uri DownloadPath { get; set; }

        /// <summary>
        ///     command to be executed after the download
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        ///     Parameters for the programm to be passed when executed
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        ///     Path to the downloaded files
        /// </summary>
        public string[] DownloadedFiles { get; set; }

        #endregion

        #region Delegates

        public delegate void CompleteEventHandler(Update update);

        public delegate void ErrorEventHandler(Update update, string errorText);

        #endregion

        #region Events

        public event CompleteEventHandler DownloadComplete;

        private void OnDownloadComplete(Update update)
        {
            DownloadComplete?.Invoke(update);
        }

        public event ErrorEventHandler DownloadError;

        private void OnDownloadError(Update update, string errorText)
        {
            DownloadError?.Invoke(update, errorText);
        }

        public event CompleteEventHandler InstallationComplete;

        private void OnInstallationComplete(Update update)
        {
            InstallationComplete?.Invoke(update);
        }

        public event ErrorEventHandler InstallationError;

        private void OnInstallationError(Update update, string errorText)
        {
            InstallationError?.Invoke(update, errorText);
        }

        #endregion

        #region Public Methods

        public void Download()
        {
            Task.Factory.StartNew(() =>
                                  {
                                      try
                                      {
                                          if (!string.IsNullOrEmpty(DownloadPath.AbsoluteUri))
                                          {
                                              string downloadedFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(DownloadPath.AbsoluteUri));
                                              using (WebClient client = new WebClient())
                                              {
                                                  client.DownloadFile(this.DownloadPath, downloadedFile);
                                                  DownloadedFiles = new string[] {downloadedFile};
                                              }
                                              OnDownloadComplete(this);
                                          }
                                          else
                                          {
                                              OnDownloadError(this, "Invalid Downloadpath");
                                          }
                                      }
                                      catch (Exception ex)
                                      {
                                          OnDownloadError(this, ex.Message);
                                      }
                                  });
        }

        public void Install()
        {
            Task.Factory.StartNew(() =>
                                  {
                                      try
                                      {
                                          if (!string.IsNullOrEmpty(this.Command))
                                          {
                                              int retVal = 0;

                                              LaunchStatus state = Launch.Execute(Command, Parameters, 1, out retVal);
                                              if (state != LaunchStatus.SUCCESS && state != LaunchStatus.TIMEOUT)
                                              {
                                                  OnInstallationError(this, Launch.LastError.Message);
                                              }
                                              else
                                                  OnInstallationComplete(this);
                                          }
                                          else
                                          {
                                              OnInstallationError(this, "No install command issued");
                                          }
                                      }
                                      catch (Exception ex)
                                      {
                                          OnInstallationError(this, ex.Message);
                                      }
                                  });
        }

        #endregion
    }
}
