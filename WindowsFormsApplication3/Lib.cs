using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace WindowsFormsApplication3
{
    class Lib
    {
        /// <summary>
        /// Проверка на доступность какой либо дериктории
        /// </summary>
        /// <param name="Directory">Конкретная дериктория</param>
        /// <returns> true - доступ есть , false - доступа нет </returns>
        public static bool AccessToFolder(DirectoryInfo Directory)
        {
            try
            {
                Directory.GetDirectories();
                return true;
            }
            catch (Exception) { return false; }
        }
        public static bool AccessToFolder(string dirName)
        {
            DirectoryInfo Directory = new DirectoryInfo(dirName);
            try
            {
                Directory.GetDirectories();
                return true;
            }
            catch (Exception) { return false; }
        }
        public static string GetName(FileInfo File)
        {
            return File.Name;
        }
        public static string GetName(DriveInfo Drive)
        {
            return Drive.Name;
        }
        public static string GetSize(FileInfo File)
        {
            if(File.Length / 1000000000000 <1000 && File.Length / 1000000000000 > 1)
            {
                return File.Length / 1000000000000 + "Tb";
            }
            else if (File.Length / 1000000000 < 1000 && File.Length / 1000000000 > 1)
            {
                return File.Length / 1000000000 + "Gb";
            }
            else if (File.Length / 1000000 < 1000 && File.Length / 1000000 > 1)
            {
                return File.Length / 1000000 + "Mb";
            }
            else if(File.Length/1000 < 1000 && File.Length / 1000 > 1)
            {
                return File.Length / 1000 + "Kb";
            }
            else 
            {
                return File.Length + "b";
            }
        }
        public static string GetSize(DriveInfo Drive)
        {
            try {
                if (Drive.TotalSize / 1000000000000 >= 1) { return "[" + Drive.TotalSize / 1000000000000 + "Tb" + "]"; }
                else { return "["+Drive.TotalSize / 1000000000 + "Gb"+"]"; }
            }
            catch (Exception e0) { Console.WriteLine(e0);return ""; }
        }
        public static void DirectoryCopy(string sourceDirName,string destDirName , bool copySubDirs = true)
        {
            destDirName = Path.Combine (destDirName  , Path.GetFileName(sourceDirName));
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if(!dir.Exists&&!AccessToFolder(dir)){ throw new DirectoryNotFoundException(); }
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName)) { Directory.CreateDirectory(destDirName); }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
                catch (Exception) { }
            }
            if(copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                { 
                    if (AccessToFolder(subdir))
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath);
                    }
                }
            }
        }
    }
}
