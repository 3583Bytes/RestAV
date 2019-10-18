# RestAV
Antivirus Rest API Running in Docker

This is a .net core Web Rest API that will accept a multipart/from-data Post request.  This will run in a docker container that is defined in the solution but it depends on another container running ClamAV it will pull from:https://hub.docker.com/r/mkodockx/docker-clamav/

Methods:

ScanFile - Accepts a multipart/from-data file to scan and provides synchronous result:

[
    "Result",
    "Clean",
    "TotalSeconds",
    "0.0295759"
]


ScanFileAsync - Accepts a multipart/from-data file to scan and provides a GUID which can be used to asynchrounously fetch a result

[
    "GUID",
    "0cc52015-2217-4c20-be93-907fb680d6ae"
]

/api/RestAV/0cc52015-2217-4c20-be93-907fb680d6ae

[
    "Result",
    "Clean"
]