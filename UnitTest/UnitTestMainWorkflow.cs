using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Not sure if this approach will work..
/// using the SampleTestingApp for now
/// 
/// http://stackoverflow.com/questions/42395681/c-sharp-integration-test-that-calls-dll-that-calls-asp-net-webapi
/// 
/// 
/// </summary>

namespace UnitTest
{
  [TestClass]
  public class UnitTestMainWorkflow
  {
    private static string ViewerServer { get; set; }
    private static ViewerCustomProperties.Integration Target { get; set; }

    [ClassInitialize]
    public static void TestClassInitialize(TestContext context)
    {
      ViewerServer = context.Properties["ViewerServer"].ToString();
      Target = new ViewerCustomProperties.Integration(ViewerServer);
    }


    [TestMethod]
    public async Task MainWorkflow()
    {
      // check local testing files
      string dwgFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Drawing.dwg");
      Assert.IsTrue(File.Exists(dwgFile), "Test DWG file not found");

      string hierarchyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Hierarchy.json");
      Assert.IsTrue(File.Exists(hierarchyFile), "Test Hierarchy file not found");

      string propertiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestFiles\Properties.json");
      Assert.IsTrue(File.Exists(propertiesFile), "Test Properties file not found");

      // upload to integration server
      string fileId = await Target.UploadFiles(dwgFile, hierarchyFile, propertiesFile);
      Assert.IsTrue(string.IsNullOrWhiteSpace(fileId));
    }
  }
}
