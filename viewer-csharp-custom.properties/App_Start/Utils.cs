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
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ViewerCustomProperties
{
  public static class Utils
  {
    /// <summary>
    /// Read config information on the web.config file
    /// </summary>
    /// <param name="settingKey"></param>
    /// <returns></returns>
    public static string GetAppSetting(string settingKey)
    {
      return WebConfigurationManager.AppSettings[settingKey];
    }

    /// <summary>
    /// Base64 encode a string (source: http://stackoverflow.com/a/11743162)
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Base64Encode(this string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Base64 dencode a string (source: http://stackoverflow.com/a/11743162)
    /// </summary>
    /// <param name="base64EncodedData"></param>
    /// <returns></returns>
    public static string Base64Decode(this string base64EncodedData)
    {
      var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
    {
      TwoLeggedApi apiInstance = new TwoLeggedApi();
      string grantType = "client_credentials";
      dynamic bearer = await apiInstance.AuthenticateAsync(WebConfigurationManager.AppSettings["FORGE_CLIENT_ID"], WebConfigurationManager.AppSettings["FORGE_CLIENT_SECRET"], grantType, scopes);
      return bearer;
    }
  }
}