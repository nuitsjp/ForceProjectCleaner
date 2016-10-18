using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ForceProjectCleaner
{
    public class Logger
    {
        private static readonly Lazy<Logger> Lazy = new Lazy<Logger>(CreateInstance);

        public static Logger Instance => Lazy.Value;

        private readonly OutputWindow _outputWindow;

        private Logger(OutputWindow outputWindow)
        {
            _outputWindow = outputWindow;
        }


        private static Logger CreateInstance()
        {
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var window = dte.Windows.Item(Constants.vsWindowKindOutput);
            var outputWindow = (OutputWindow)window.Object;
            outputWindow.ActivePane?.Activate();
            return new Logger(outputWindow);
        }

        public void WriteLog(Exception e)
        {
            WriteLog($"Exception:{e.GetType().FullName} Message:{e.Message} StackTrace:{e.StackTrace}");
        }

        public void WriteLog(string message)
        {
            if (_outputWindow.ActivePane != null)
            {
                _outputWindow.ActivePane.Activate();
                _outputWindow.ActivePane.OutputString($"ForceProjectCleaner {message}{Environment.NewLine}");
            }
        }

    }
}
