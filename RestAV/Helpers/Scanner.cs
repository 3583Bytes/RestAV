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



        public static void Verify()
        {
            ClamAVVerify();
        }

        private static async void ClamAVVerify()
        {
            ClamClient clam = new ClamClient(ClamAVServer, ClamAVPort);

            bool result = await clam.PingAsync();

            if (result)
            {
                ClamAVStatus = "ClamAV Running";
            }
            else
            {
                ClamAVStatus = "Failed to Communicate with ClamAV";
            }

        }

        public static Guid ScanFileAsync(string filePath)
        {
            Guid fileGUID = Guid.NewGuid();

            ScanHistory.Add(new ScanResults(fileGUID));

            ClamAVScanFile(filePath, fileGUID);

            return fileGUID;
        }

        public static Guid ScanBytesAsync(byte[] fileData)
        {
            Guid fileGUID = Guid.NewGuid();

            ScanHistory.Add(new ScanResults(fileGUID));

            ClamAVScanBytes(fileData, fileGUID);

            return fileGUID;
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

        private static async void ClamAVScanFile(string filePath, Guid fileGUID)
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

        private static async void ClamAVScanBytes(byte[] fileData, Guid fileGUID)
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
