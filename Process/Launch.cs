using System;
using System.Diagnostics;

namespace Aurora.Misc.Process
{
    public enum LaunchStatus
    {
        ERROR = 0,
        SUCCESS = 1,
        TIMEOUT = 2
    }

    public class Launch
    {
        public static Exception LastError;

        /// <summary>
        ///     Launch a command with the given parameters and wait for its execution to complete
        /// </summary>
        /// <param name="command">Commandstring for the launch</param>
        /// <param name="parameter">parameter for the execution</param>
        /// <param name="retVal">Returncode from the completed command</param>
        /// <returns>Information about the launched command</returns>
        public static LaunchStatus Execute(string command, string parameter, out int retVal)
        {
            return (Execute(command, parameter, 0, out retVal));
        }

        /// <summary>
        ///     Launch a command with the given parameter
        /// </summary>
        /// <param name="command">Commandstring for the launch</param>
        /// <param name="parameter">configData to be passed to the launched command</param>
        /// <param name="timeout">Timeout to wait for the commandexecution. if 0 wait indefinitly</param>
        /// <param name="retVal">Returncode from the completed command</param>
        /// <returns>Information about the launched command</returns>
        public static LaunchStatus Execute(string command, string parameter, int timeout, out int retVal)
        {
            bool completed = true;
            LaunchStatus reply = LaunchStatus.SUCCESS;

            retVal = 0;
            try
            {
                System.Diagnostics.Process kopierer = new System.Diagnostics.Process();
                command = Environment.ExpandEnvironmentVariables(command);
                parameter = Environment.ExpandEnvironmentVariables(parameter);
                kopierer.StartInfo = new ProcessStartInfo(command);
                kopierer.StartInfo.Arguments = parameter;

                kopierer.Start();

                if (timeout > 0)
                    completed = kopierer.WaitForExit(timeout);
                else
                    kopierer.WaitForExit();

                if (!completed && !kopierer.HasExited)
                {
                    reply = LaunchStatus.TIMEOUT;
                    //Kopierer.Kill();
                }

                retVal = kopierer.ExitCode;
                if (retVal != 0)
                    reply = LaunchStatus.ERROR;
            }
            catch (Exception ex)
            {
                reply = LaunchStatus.ERROR;
                LastError = ex;
            }
            return (reply);
        }
    }
}
