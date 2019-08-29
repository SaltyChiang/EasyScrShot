using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EasyScrShot.HelperLib
{
    public class OptiPNG
    {
        public static class OptimizationLevel
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
        
        public OptiPNG()
        {
            Arguments += OptimizationLevel.Level1;
        }
        public OptiPNG(string optimizationLevel)
        {
            Arguments += optimizationLevel;
        }

        public void LosslessCompress(string inputFile, string outputFile)
        {
            Arguments += " -nx";
            Arguments += " " + "\"" + inputFile + "\"";
            Arguments += " -out " + "\"" + outputFile + "\"";
            Process process = new Process();
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
}
