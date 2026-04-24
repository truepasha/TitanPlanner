using System;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();

            string strVersion = typeof(Splash).GetType().Assembly.GetName().Version.ToString();

            TXT_version.Text = "Version: v" + Application.ProductVersion; // +" Build " + strVersion;

            var customSplash = Path.Combine(Settings.GetRunningDirectory(), "splash.png");
            if (File.Exists(customSplash))
            {
                startupImage.Image = System.Drawing.Image.FromFile(customSplash);
            }

            Console.WriteLine(strVersion);

            Console.WriteLine("Splash .ctor");
        }

        private void TXT_version_Click(object sender, EventArgs e)
        {

        }

        private void startupImage_Click(object sender, EventArgs e)
        {

        }

        private void startupImage_Click_1(object sender, EventArgs e)
        {

        }
    }
}