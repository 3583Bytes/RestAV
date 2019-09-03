using nClam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAV.Helpers
{
    public class ScanResults
    {
        public ClamScanResult ScanResult;
        public Guid FileGUID;


        public ScanResults(Guid fileGUID)
        {
            FileGUID = fileGUID;
        }
        public ScanResults(Guid fileGUID, ClamScanResult scanResult)
        {
            FileGUID = fileGUID;
            ScanResult = scanResult;
        }
        public ScanResults(Guid fileGUID, string error)
        {
            ScanResult = new ClamScanResult(error);
        }
    }
}
