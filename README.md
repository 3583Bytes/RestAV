# RestAV
Antivirus Rest API Running in Docker

This is a .net core Web Rest API that will accept a JSON Post request.  This uses will run in a docker container that is defined in the solution but it depends on another container running ClamAV it will pull from:https://hub.docker.com/r/mkodockx/docker-clamav/

Parameters:

Mode 0 = Synchronous 1= Asynchronous

Request Example 

{
	"Mode":"0",
	"FileData":"WDVPIVAlQEFQWzRcUFpYNTQoUF4pN0NDKTd9JEVJQ0FSLVNUQU5EQVJELUFOVElWSVJVUy1URVNULUZJTEUhJEgrSCo=",
	"FileName":"Test.txt"
}

Response Example

[
    "Result",
    "Infected",
    "File",
    "stream",
    "Virus Eicar-Test-Signature",
    "TotalSeconds",
    "0.0197741"
]
