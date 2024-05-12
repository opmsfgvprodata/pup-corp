using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateFile
    {
        public static string TextFileContent(string data, int length, string emptyReplceData, bool isLeft)
        {
            string result = "";

            if (data.Length > length)
            {
                result = data.Substring(0, length);
            }
            else
            {
                if (isLeft)
                {
                    result = data.PadLeft(length, Convert.ToChar(emptyReplceData));
                }
                else
                {
                    result = data.PadRight(length, Convert.ToChar(emptyReplceData));
                }
            }

            return result;
        }

        public string CreateTextFile(string filename, string fileContent, string subFolderName)
        {
            string folderPath = "~/TextFile/" + subFolderName + "/";
            string path = HttpContext.Current.Server.MapPath(folderPath);
            string filecreation = path + filename;

            TryToDelete(filecreation);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                writer.WriteLine(fileContent);
                writer.Close();
            }

            return folderPath;
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}