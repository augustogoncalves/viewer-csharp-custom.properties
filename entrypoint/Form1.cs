using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace entrypoint
{
  public partial class Form1 : Form
  {
    private static ViewerCustomProperties.Integration Target { get; set; }

    public Form1()
    {
      InitializeComponent();
      Target = new ViewerCustomProperties.Integration(ConfigurationManager.AppSettings["ViewerServer"]);
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
      // check local testing files
      string dwgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Drawing.dwg");
      string hierarchyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Hierarchy.json");
      string propertiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Properties.json");

      // upload to integration server
      string fileId = await Target.UploadFiles(dwgFile, hierarchyFile, propertiesFile);

      Process.Start(Target.BaseURL + "/?m=" + fileId);

      Close();
    }
  }
}
