using System;
using System.Windows.Forms;
using System.Threading;
using EasyScrShot;
using EasyScrShot.HelperLib;

namespace EasyScrShot
{
    public partial class VSInfoWindow : Form
    {
        private string[] fileList { get; set; }

        public VSInfoWindow(string[] fileList)
        {
            InitializeComponent();
            this.fileList = fileList;
        }

        public Info result { get; private set; }

        private void YesButton_Click(object sender, EventArgs e)
        {
            int N = int.Parse(BoxN.Text),
                s = int.Parse(Boxs.Text),
                r = int.Parse(Boxr.Text);
            result = new VSInfo(N, s, r);
            this.Close();
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            result = new AVSInfo();
            this.Close();
        }

        private void CompButton_Click(object sender, EventArgs e)
        {
            this.CompButton.Enabled = false;
            this.YesButton.Enabled = false;
            this.NoButton.Enabled = false;
            Thread thread = new Thread(() =>
            {
                ThreadCompButton_Click(fileList);
            });
            thread.Start();

            var processWindow = new ProgressWindow(fileList.Length);
            processWindow.Show(this);
            int completed = 0;
            while (completed < fileList.Length)
            {
                processWindow.SetBar(completed);
                Thread.Sleep(100);
                completed = PNGHelpers.GetCompletedCount();
            }
            processWindow.Close();
            this.YesButton.Enabled = true;
            this.NoButton.Enabled = true;
        }

        private void ThreadCompButton_Click(string[] fileList)
        {
            Thread thread = new Thread(() =>
            {
                PNGHelpers.MultiThreadPNGCompress(fileList);
            });
            thread.Start();
            thread.Join();
        }
    }
}
