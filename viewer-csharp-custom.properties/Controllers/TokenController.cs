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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ViewerCustomProperties.Controllers
{
  public class TokenController : ApiController
  {
    [HttpGet]
    [Route("api/forge/oauth/token")]
    public async Task<HttpResponseMessage> GetToken()
    {
      // get a read-only token
      // NEVER return write-enabled token to client
      dynamic oauth = await Utils.Get2LeggedTokenAsync(new Scope[] { Scope.DataRead });
      return Request.CreateResponse(HttpStatusCode.OK, new { access_token = oauth.access_token, expires_in = oauth.expires_in });
    }
  }
}
