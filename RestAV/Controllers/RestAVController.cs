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
        public ActionResult<IEnumerable<string>> GetAsych()
        {
            try
            {
                Task.WaitAll(Scanner.Verify());

            }
            catch (Exception exception)
            {
                Scanner.ClamAVStatus = exception.Message;
            }

            
            return new string[] { "RestAV API v1.0", Scanner.ClamAVStatus };
        }

        // GET api/RestAV/{GUID}
        [HttpGet("{guid}")]
        public ActionResult<string[]> Get(string guid)
        {
            if (guid is null)
            {
                return new string[] { "Error", "Missing GUID" };
            }

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

        // GET api/RestAV/FileAsync
        [HttpPost("ScanFileAsync")]
        public string[] PostAsync([FromForm] IFormFile ScanFile, [FromForm] String APIKey)
        {
            if (ScanFile is null)
            {
                return new string[] { "Error", "Missing ScanFile" };
            }

            if (ScanFile.Length > 0)
            {
                Guid fileGUID = Guid.NewGuid();

                //Add to History so we can update it once scan is done
                Scanner.ScanHistory.Add(new ScanResults(fileGUID));

                //Scan Stream
                Scanner.ScanStream(ScanFile.OpenReadStream(), fileGUID).Wait();
                
                //Return GUID
                return new string[] { "GUID", fileGUID.ToString() };
               
            }

            return new string[] { "Error ", "No File Stream Found" };
        }

        // GET api/RestAV/FileSync
        [HttpPost("ScanFile")]
        public string[] Post([FromForm] IFormFile ScanFile, [FromForm] String APIKey)
        {
            if (ScanFile is null)
            {
                return new string[] { "Error", "Missing ScanFile" };
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (ScanFile.Length > 0)
            {
                Guid fileGUID = Guid.NewGuid();

                //Add to History so we can update it once scan is done
                Scanner.ScanHistory.Add(new ScanResults(fileGUID));

                Task.WaitAll(Scanner.ScanStream(ScanFile.OpenReadStream(), fileGUID));

                ScanResults result = Scanner.ScanHistory.Find(x => x.FileGUID == fileGUID);

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

            return new string[] { "Error ", "Invalid ScanFile" };
        }

    }
}