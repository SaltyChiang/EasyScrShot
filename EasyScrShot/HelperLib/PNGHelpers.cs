using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace EasyScrShot.HelperLib
{
    public class OptiPNG
    {
        internal static class OptimizationLevel
        {
            public const string Level0 = "-o0",
                Level1 = "-o1",
                Level2 = "-o2",
                Level3 = "-o3",
                Level4 = "-o4",
                Level5 = "-o5",
                Level6 = "-o6",
                Level7 = "-o7",
                Level8 = "-o8",
                Level9 = "-o9";
        };

        private string Arguments = "-nx";

        internal OptiPNG()
        {
            Arguments += " " + OptimizationLevel.Level1;
        }
        internal OptiPNG(string optimizationLevel)
        {
            Arguments += " " + optimizationLevel;
        }

        internal void LosslessCompress(string inputFile, string outputFile)
        {
            Arguments += " " + "\"" + inputFile + "\"";
            Arguments += " -out " + "\"" + outputFile + "\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "./optipng";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = Arguments;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }

    public static class PNGHelpers
    {
        private static int completeCount { get; set; }
        private static int fileCount { get; set; }

        public static void MultiThreadPNGCompress(string[] fileList)
        {
            MultiThreadPNGCompress(fileList, Environment.ProcessorCount);
        }

        public static void MultiThreadPNGCompress(string[] fileList, int threadsMaxCount)
        {
            int i, j;
            fileCount = fileList.Length;
            PreCompress();
            if (fileCount < threadsMaxCount)
                threadsMaxCount = fileCount;

            Task[] tasks = new Task[threadsMaxCount];
            Action<object> action = (object obj) =>
                {
                    PNGCompress(obj);
                };

            for (i = 0; i < fileCount; i++)
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

            //RemoveTemp("optipng.exe");
        }

        private static void PNGCompress(object fileNameObject)
        {
            PNGCompress(fileNameObject.ToString());
        }

        private static void PNGCompress(string fileName)
        {
            OptiPNG optiPNG = new OptiPNG();
            optiPNG.LosslessCompress(fileName, "temp." + fileName);
            completeCount++;
        }

        private static void PreCompress()
        {

            completeCount = 0;
        }

        private static void RemoveTemp(string tempFile)
        {
            FileInfo file = new FileInfo(tempFile);
            file.Delete();
        }

        public static int GetCompletedCount()
        {
            return completeCount;
        }
    }
}
