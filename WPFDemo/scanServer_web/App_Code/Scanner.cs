//////////////////////////////////////////////////////////////////
//
// Copyright (c) 2011, IBN Labs, Ltd All rights reserved.
// Please Contact rnd@ibn-labs.com 
//
// This Code is released under Code Project Open License (CPOL) 1.02,
//
// To emphesize - # Representations, Warranties and Disclaimer. THIS WORK IS PROVIDED "AS IS", "WHERE IS" AND "AS AVAILABLE", WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES OR CONDITIONS OR GUARANTEES. YOU, THE USER, ASSUME ALL RISK IN ITS USE, INCLUDING COPYRIGHT INFRINGEMENT, PATENT INFRINGEMENT, SUITABILITY, ETC. AUTHOR EXPRESSLY DISCLAIMS ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR CONDITIONS, INCLUDING WITHOUT LIMITATION, WARRANTIES OR CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY OR FITNESS FOR A PARTICULAR PURPOSE, OR ANY WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY PORTION THEREOF) IS CORRECT, USEFUL, BUG-FREE OR FREE OF VIRUSES. YOU MUST PASS THIS DISCLAIMER ON WHENEVER YOU DISTRIBUTE THE WORK OR DERIVATIVE WORKS.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;

/// <summary>
/// Summary description for Scanner
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Scanner : System.Web.Services.WebService
{
    public Scanner()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public string UploadScan(byte[] data, string scanKey)
    {
        Guid fileID = Guid.NewGuid();
        string path = Server.MapPath("~/Documents");
        string filePath = path + "/" + fileID.ToString() + ".jpg";
        try {
            // TBD: folder by date of upload to prevent too many files in the uploads folder
            FileStream traget = new FileStream(filePath, FileMode.Create); // the jpg extensio is for debug
            traget.Write(data, 0, data.Length);
            traget.Flush();
            traget.Close();
            // TBD: register in the DB (fileID, scanKey, person, date etc)
        } catch (Exception ex) {
            // TBD: cleanup - delete file, clear DB atc.
            // TBD: register the error and alert operators
            return "Error: " + ex.Message;
        }
        return "Saved";
    }

}
