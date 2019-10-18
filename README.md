# RestAV
.Net core REST API providing methods to scan files with ClamAV Antivirus running in Docker.

This is a .Net core Web Rest API that will accept a multipart/from-data Post request.  

This will run in a docker container that is defined in [docker-compose.yml](https://github.com/3583Bytes/RestAV/blob/master/docker-compose.yml) which has a dependency on [ClamAV](https://hub.docker.com/r/mkodockx/docker-clamav/) container.

## RestAV API Methods:

### Post:

*/api/RestAV/ScanFile*: Accepts a multipart/from-data file to scan and provides synchronous result:
Returns:
```
[
    "Result",
    "Clean",
    "TotalSeconds",
    "0.0295759"
]
```


*/api/RestAV/ScanFileAsync*: Accepts a multipart/from-data file to scan and provides a GUID which can be used to asynchrounously fetch a result
Returns:
```
[
    "GUID",
    "0cc52015-2217-4c20-be93-907fb680d6ae"
]
```

### Get:

*/api/RestAV/{GUID}*

```
[
    "Result",
    "Clean"
]
```

## Run:

build solution then run 

```
docker-compose up
```

FYI: Running the solution in Docker via Visual Studio will only run the RestAV docker image withough the require ClamAV container.  Hence the need to run docker-compose up.

## Test:

[Postman Collection](https://github.com/3583Bytes/RestAV/blob/master/RestAV.postman_collection.json) provided for testing.



