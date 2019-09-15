using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Gears.Resources
{
    public class FileResourceExtention : Xamarin.Forms.Xaml.IMarkupExtension
    {
        public string Source { get; set; }
        public static void CopyToLocal() {
            var assembly = Assembly.GetExecutingAssembly();
            var local = LocalCopyPath;
            var targetFolder = Directory.CreateDirectory(Path.Combine(local, "www")).FullName;
            var allFiles = assembly.GetManifestResourceNames();
            foreach (var sourcePath in allFiles)
            {
                if (sourcePath.Contains("www"))
                {
                    var splited = new List<string>(sourcePath.Split('.'));
                    splited.RemoveAt(0);
                    splited.RemoveAt(0);
                    splited.RemoveAt(0);
                    var subPath = String.Join(".", splited);
                    var targetPath = Path.Combine(targetFolder, subPath);
                    if (File.Exists(targetPath))
                        File.Delete(targetPath);
                    var targetStream = File.Create(targetPath);
                    var sourceStream = assembly.GetManifestResourceStream(sourcePath);
                    sourceStream.CopyTo(targetStream);
                    targetStream.Close();
                    sourceStream.Close();
                }
            }
        }

        public static string getUrl(string fileName)
        {
            var path = "file:///" + Path.Combine(LocalCopyPath, "www", fileName).Replace("\\", "/");
            return path;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null )
            {
                return null;
            }

            return getUrl(Source);
        }

        public static string LocalCopyPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
