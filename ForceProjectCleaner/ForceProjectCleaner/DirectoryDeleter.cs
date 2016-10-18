using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceProjectCleaner
{
    public static class DirectoryDeleter
    {
        public static void ForceDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            try
            {
                Logger.Instance.WriteLog($"Delete {path}");
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLog(ex);
            }
        }

    }
}
