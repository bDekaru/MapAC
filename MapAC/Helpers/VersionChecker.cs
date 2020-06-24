using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAC.Helpers
{
    public class VersionChecker
    {

        JObject archive;

        private bool CheckVersion(string filename, string iteration)
        {
            if(archive == null)
                archive = JObject.Parse(File.ReadAllText(@"archive.json"));

            if (archive.ContainsKey(filename))
            {
                var dat = archive[filename];
                if (dat[iteration] != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// For Cell files, returns if the file in the archive is "complete"
        /// </summary>
        /// <returns></returns>
        public bool IsComplete(string filename, string iteration)
        {
            if (archive == null)
                archive = JObject.Parse(File.ReadAllText(@"archive.json"));

            if (archive.ContainsKey(filename))
            {
                var dat = archive[filename];
                if (dat[iteration] != null && dat[iteration].Contains("complete")) 
                {
                    if(dat[iteration]["complete"].ToString() != "100")
                        return true;
                }
            }
            return false;
        }

        public string GetVersionInfo(string filename, string iteration)
        {
            filename = filename.ToLower(); // convert to lower case as that's the format in our json
            if(CheckVersion(filename, iteration))
            {
                return archive[filename][iteration]["date"].ToString();
            }
            return "";
        }

    }
}
