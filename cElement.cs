using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Сache_Сleaner
{
    class cElement
    {
        public string Name { set; get; }
        public Guid guid { set; get; }
        public string Folder { set; get; }
        public bool isFolder { set; get; }
        public string Connect { set; get; }
        public List<string> Pathes
        { 
            get 
            {
                List<string> p = new List<string>();
                p.Add(Environment.GetEnvironmentVariable("AppData") + "\\1C\\1cv8\\" + guid.ToString());
                p.Add(Environment.GetEnvironmentVariable("AppData") + "\\..\\Local\\1C\\1cv8\\" + guid.ToString());
                p.Add(Environment.GetEnvironmentVariable("AppData") + "\\1C\\1cv82\\" + guid.ToString());
                p.Add(Environment.GetEnvironmentVariable("AppData") + "\\..\\Local\\1C\\1cv82\\" + guid.ToString());

                return p;
            }
        }

        public long CacheSize
        {
            
            get
            {
                long size =  0;
                foreach(string Path in Pathes)
                    size += SafeEnumerateFiles(Path, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
                return size;
            }
        }

        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDirPath = dirs.Pop();
                if (searchOption == SearchOption.AllDirectories)
                {
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(currentDirPath);
                        foreach (string subDirPath in subDirs)
                        {
                            dirs.Push(subDirPath);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string filePath in files)
                {
                    yield return filePath;
                }
            }
        }

        public cElement(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }




        public void ClearCache()

        {
            foreach (string path in Pathes)
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
        }

    }
}
