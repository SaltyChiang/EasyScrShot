using System;
using System.IO;
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

        private string Arguments;

        internal OptiPNG() :
            this(OptimizationLevel.Level1)
        {
        }
        internal OptiPNG(string optimizationLevel)
        {
            Arguments = $" {optimizationLevel}";
        }

        internal void LosslessCompress(string inputFile)
        {
            Arguments += $" -preserve \"{inputFile}\" -backup";
            ExecuteOptiPNG();
        }

        internal void LosslessCompress(string inputFile, string outputFile)
        {
            if (inputFile.Equals(outputFile))
                Arguments += $" -preserve \"{inputFile}\" -backup";
            else
                Arguments += $" -preserve \"{inputFile}\" -out \"{outputFile}\"";
            ExecuteOptiPNG();
        }

        private void ExecuteOptiPNG()
        {
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
            optiPNG.LosslessCompress(fileName);
            CompleteCount++;
        }

        private static void PreCompress()
        {
            byte[] asm = EasyScrShot.Properties.Resources.optipng;
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
