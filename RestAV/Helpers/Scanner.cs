using nClam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAV.Helpers
{
    public static class Scanner
    {
        public static List<ScanResults> ScanHistory = new List<ScanResults>();
        public static int ClamAVPort = 3310;
        public static string ClamAVServer = "localhost";
        public static string ClamAVStatus = "";

        /// <summary>
        /// Verify that ClamAV is running
        /// </summary>
        /// <returns></returns>
        public static async Task Verify()
        {
            ClamClient clam = new ClamClient(ClamAVServer, ClamAVPort);

            bool result = await clam.PingAsync();

            if (result)
            {
                ClamAVStatus = "Clam AV Running";
            }
            else
            {
                ClamAVStatus = "Failed to Communicate with Clam AV";
            }

        }

        public static void DisplayResults(Guid fileGUID)
        {
            ScanResults result = ScanHistory.Find(x => x.FileGUID == fileGUID);

            switch (result.ScanResult.Result)
            {
                case ClamScanResults.Clean:
                    Console.WriteLine("The file is clean!");
                    break;
                case ClamScanResults.VirusDetected:
                    Console.WriteLine("Virus Found!");
                    Console.WriteLine("Virus name: {0}", result.ScanResult.InfectedFiles.First().VirusName);
                    Console.WriteLine("File name: {0}", result.ScanResult.InfectedFiles.First().FileName);
                    break;
                case ClamScanResults.Error:
                    Console.WriteLine("Woah an error occured! Error: {0}", result.ScanResult.RawResult);
                    break;
            }
        }

        /// <summary>
        /// Scans a local file, this is not used since the file has to be localy available on the server
        /// </summary>
        /// <param name="filePath">Local path to the file to be scanned</param>
        /// <param name="fileGUID">The Results GUID used to index for scan result</param>
        /// <returns></returns>

        private static async Task ScanFile(string filePath, Guid fileGUID)
        {
            try
            {
                ClamClient clam = new ClamClient(ClamAVServer, ClamAVPort);

                var scanResult = await clam.ScanFileOnServerAsync(filePath);  //any file you would like!

                ScanResults result = ScanHistory.Find(x => x.FileGUID == fileGUID);

                result.ScanResult = scanResult;
            }
            catch (Exception exception)
            {
                ScanResults result = ScanHistory.Find(x => x.FileGUID == fileGUID);

                if (result == null)
                {
                    result = new ScanResults(fileGUID, exception.Message);
                }
                else
                {
                    result.ScanResult = new ClamScanResult(exception.Message);
                }

            }
        }

        /// <summary>
        /// Scans an array of Base64 encoded bytes
        /// </summary>
        /// <param name="fileData">The Data to be scanned</param>
        /// <param name="fileGUID">The Results GUID used to index for scan result</param>
        /// <returns></returns>
        public static async Task ScanBytes(byte[] fileData, Guid fileGUID)
        {
            try
            {
                ClamClient clam = new ClamClient(ClamAVServer, ClamAVPort);

                var scanResult = await clam.SendAndScanFileAsync(fileData);  

                ScanResults result = ScanHistory.Find(x => x.FileGUID == fileGUID);

                result.ScanResult = scanResult;
            }
            catch (Exception exception)
            {
                ScanResults result = ScanHistory.Find(x => x.FileGUID == fileGUID);

                if (result == null)
                {
                    result = new ScanResults(fileGUID, exception.Message);
                }
                else
                {
                    result.ScanResult = new ClamScanResult(exception.Message);
                }

            }
        }


    }
}
