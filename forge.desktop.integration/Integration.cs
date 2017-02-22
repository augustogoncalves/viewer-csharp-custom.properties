/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ViewerCustomProperties
{
  public class Integration
  {
    private struct EndPoints
    {
      public const string Upload = "api/integration/uploadFiles";
    }

    public string BaseURL { get; private set; }

    public Integration(string baseURL)
    {
      BaseURL = baseURL;
    }

    public async Task<String> UploadFiles(string fileToTranslate, string hierarchyFile, string propertiesFile)
    {
      var client = new RestClient(BaseURL);
      var request = new RestRequest(EndPoints.Upload, Method.PUT);
      request.AddFile("FileToTranslate", File.ReadAllBytes(fileToTranslate), Path.GetFileName(fileToTranslate));
      request.AddFile("Hierarchy", File.ReadAllBytes(hierarchyFile), Path.GetFileName(hierarchyFile));
      request.AddFile("Properties", File.ReadAllBytes(propertiesFile), Path.GetFileName(propertiesFile));
      IRestResponse response = await client.ExecuteTaskAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
        throw new System.Exception("Error uploading file: " + response.StatusCode);

      return JsonConvert.DeserializeObject<string>(response.Content);
    }
  }
}
