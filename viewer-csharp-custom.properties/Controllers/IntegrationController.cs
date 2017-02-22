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

using Autodesk.Forge;
using Autodesk.Forge.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ViewerCustomProperties.Controllers
{
  public class IntegrationController : ApiController
  {
    [HttpPut]
    [Route("api/integration/uploadFiles")]
    public async Task<string> UploadFiles()
    {
      HttpRequest req = HttpContext.Current.Request;

      // we must have 3 file on the request
      if (req.Files.Count != 3) throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unexpected number of files"));
      HttpPostedFile drawingFile = req.Files["FileToTranslate"];
      HttpPostedFile hierarchyFile = req.Files["Hierarchy"];
      HttpPostedFile propertiesFile = req.Files["Properties"];

      // saves files to /App_Data/
      string bucketKey = Path.GetFileNameWithoutExtension(drawingFile.FileName).ToLower() + DateTime.Now.ToString("-yyyyMMddHHmmssfff");
      string drawingFilePath = SaveToFolder(drawingFile, bucketKey);
      SaveToFolder(hierarchyFile, bucketKey);
      SaveToFolder(propertiesFile, bucketKey);

      try
      {
        // authenticate with Forge
        dynamic oauth = await Utils.Get2LeggedTokenAsync(new Scope[] { Scope.BucketCreate, Scope.DataRead, Scope.DataCreate, Scope.DataWrite });

        // create the bucket
        BucketsApi buckets = new BucketsApi();
        buckets.Configuration.AccessToken = oauth.access_token;
        PostBucketsPayload bucketPayload = new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Transient /* erase after 24h */);
        dynamic bucketResult = await buckets.CreateBucketAsync(bucketPayload);

        // upload the file/object, which will create a new object
        ObjectsApi objects = new ObjectsApi();
        objects.Configuration.AccessToken = oauth.access_token;
        dynamic uploadedObj;
        using (StreamReader streamReader = new StreamReader(drawingFilePath))
        {
          uploadedObj = await objects.UploadObjectAsync(bucketKey,
                 drawingFile.FileName, (int)streamReader.BaseStream.Length, streamReader.BaseStream,
                 "application/octet-stream");
        }

        // start translating the file
        List<JobPayloadItem> outputs = new List<JobPayloadItem>()
        {
         new JobPayloadItem(
           JobPayloadItem.TypeEnum.Svf,
           new List<JobPayloadItem.ViewsEnum>()
           {
             JobPayloadItem.ViewsEnum._2d,
             JobPayloadItem.ViewsEnum._3d
           })
        };
        JobPayload job = new JobPayload(new JobPayloadInput(Utils.Base64Encode(uploadedObj.objectId)), new JobPayloadOutput(outputs));
        DerivativesApi derivative = new DerivativesApi();
        derivative.Configuration.AccessToken = oauth.access_token;
        dynamic jobPosted = await derivative.TranslateAsync(job);

        return bucketKey;
      }
      catch (System.Exception ex)
      {
        // for this testing app, let's throw a full descriptive expcetion,
        // which is not a good idea in production
        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message + ex.InnerException));
      }     
    }

    private string SaveToFolder(HttpPostedFile file, string folder)
    {
      var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), folder, file.FileName);
      if (!Directory.Exists(fileSavePath)) Directory.CreateDirectory(Path.GetDirectoryName( fileSavePath));
      file.SaveAs(fileSavePath);
      return fileSavePath;
    }
  }
}
