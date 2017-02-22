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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ViewerCustomProperties.Controllers
{
  public class ModelsController : ApiController
  {
    [HttpGet]
    [Route("api/viewer/urn")]
    public async Task<HttpResponseMessage> GetModelURN(string bucketKey)
    {
      dynamic oauth = await Utils.Get2LeggedTokenAsync(new Scope[] { Scope.BucketRead, Scope.DataRead });

      ObjectsApi objects = new ObjectsApi();
      objects.Configuration.AccessToken = oauth.access_token;
      var objectsList = objects.GetObjects(bucketKey);
      string urn = string.Empty;
      foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
      {
        // for this sample, each bucket will have only 1 object
        urn = ((string)objInfo.Value.objectId).Base64Encode();
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { urn = urn });
    }

    [HttpGet]
    [Route("api/viewer/properties")]
    public async Task<HttpResponseMessage> GetProperties(string bucketKey)
    {
      string jsonPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), bucketKey, "properties.json");
      using (StreamReader r = new StreamReader(jsonPath))
      {
        string json = r.ReadToEnd();
        return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(json));
      }
    }
  }
}
