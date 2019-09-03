using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nClam;
using RestAV.Helpers;

namespace RestAV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestAVController : ControllerBase
    {
        // GET api/RestAV
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            Scanner.Verify();

            do
            {
                //Wait for Verify Async Method to Finish

            } while (String.IsNullOrEmpty(Scanner.ClamAVStatus));


            return new string[] { "RestAV API v1.0", Scanner.ClamAVStatus };
        }


        // GET api/RestAV/5
        [HttpGet("{guid}")]
        public ActionResult<string[]> Get(string guid)
        {
            Guid fileGUID = new Guid(guid);

            ScanResults result = Scanner.ScanHistory.Find(x => x.FileGUID == fileGUID);

            if (result != null)
            {
                switch (result.ScanResult.Result)
                {
                    case ClamScanResults.Clean:
                        return new string[] { "Result", "Clean" };

                    case ClamScanResults.VirusDetected:
                        return new string[] { "Result", "Infected", "File", result.ScanResult.InfectedFiles.First().FileName.Replace("\\\\?\\", ""), "Virus" + result.ScanResult.InfectedFiles.First().VirusName };

                    case ClamScanResults.Error:
                        return new string[] { "Result", "Error", "Details", result.ScanResult.RawResult.Replace("\\\\?\\", "") };

                }
            }
            return new string[] { "Unavailable", guid.ToString() };
        }

        // POST api/RestAV
        [HttpPost]
        public string[] Post([FromBody] FileScanInfo fileScanInfo)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            Guid fileGUID = new Guid();

            //fileGUID = Scanner.ScanFileAsync(fileScanInfo.FilePath);

            fileGUID = Scanner.ScanBytesAsync(fileScanInfo.FileData);


            ScanResults result = Scanner.ScanHistory.Find(x => x.FileGUID == fileGUID);

            if (fileScanInfo.Mode == 1)
            {
                return new string[] { "GUID", fileGUID.ToString() };
            }

            do
            {
                //Wait for Verify Scan Method to Finish
            } while (result.ScanResult == null);

            watch.Stop();

            switch (result.ScanResult.Result)
            {
                case ClamScanResults.Clean:
                    return new string[] { "Result", "Clean", "TotalSeconds", watch.Elapsed.TotalSeconds.ToString() };

                case ClamScanResults.VirusDetected:
                    return new string[] { "Result", "Infected", "File", result.ScanResult.InfectedFiles.First().FileName.Replace("\\\\?\\", ""), "Virus" + result.ScanResult.InfectedFiles.First().VirusName, "TotalSeconds", watch.Elapsed.TotalSeconds.ToString() };

                case ClamScanResults.Error:
                    return new string[] { "Result", "Error", "Details", result.ScanResult.RawResult.Replace("\\\\?\\", ""), "TotalSeconds", watch.Elapsed.TotalSeconds.ToString() };

            }

            return new string[] { "Scanning ", fileGUID.ToString() };
        }
    }
}