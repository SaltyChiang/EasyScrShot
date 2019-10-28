using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace EasyScrShot.HelperLib
{
    public static class PNGHelpers
    {
        private static int CompleteCount { get; set; }
        private static int FileCount { get; set; }

        public static void MultiThreadPNGCompress(string[] fileList)
        {
            MultiThreadPNGCompress(fileList, Environment.ProcessorCount);
        }

        public static void MultiThreadPNGCompress(string[] fileList, int threadsMaxCount)
        {
            int i, j;
            FileCount = fileList.Length;
            PreCompress();
            if (FileCount < threadsMaxCount)
                threadsMaxCount = FileCount;

            Task[] tasks = new Task[threadsMaxCount];
            Action<object> action = (object obj) =>
                {
                    PNGCompress(obj);
                };

            for (i = 0; i < FileCount; i++)
            {
                if (i < threadsMaxCount)
                {
                    tasks[i] = new Task(action, fileList[i]);
                    tasks[i].Start();
                }
                else
                {
                    j = Task.WaitAny(tasks);
                    tasks[j] = new Task(action, fileList[i]);
                    tasks[j].Start();
                }
            }
            Task.WaitAll(tasks);

            RemoveTemp("optipng.exe");
        }

        private static void PNGCompress(object fileNameObject)
        {
            PNGCompress(fileNameObject.ToString());
        }

        private static void PNGCompress(string fileName)
        {
            OptiPNG optiPNG = new OptiPNG();
            optiPNG.LosslessCompress(fileName, "temp." + fileName);
            CompleteCount++;
        }

        private static void PreCompress()
        {
            byte[] asm = EasyScrShot.Properties.Resources.optipng_win;
            FileStream fileStream = new FileStream("optipng.exe", FileMode.Create);
            fileStream.Write(asm, 0, asm.Length);
            fileStream.Close();
            CompleteCount = 0;
        }

        private static void RemoveTemp(string tempFile)
        {
            FileInfo file = new FileInfo(tempFile);
            file.Delete();
        }

        public static int GetCompletedCount()
        {
            return CompleteCount;
        }
    }
}
