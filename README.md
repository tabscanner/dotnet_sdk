# AI Scan Tech- dotnet sdk

A demo application and related classes used to call the AI Scan Tech Receipt OCR API and process the results using dotnet core. This demo app will upload a receipt image in the receipt_images directory to the API and return the result as a dotnet object. It will also demonstrate serialising the result to json and saving as a file in the results directory.

## Getting Started

Register at tabscanner.com for an API key.

### Prerequisites

```
dotnet core version 2
```
Optional

```
The latest version of docker if you prefer the docker way.
```

### Installing

### Visual Studio Code

Clone the project and add as a directory in Visual Studio Code

```
dotnet restore
dotnet build
```

### Docker

Clone the project and cd to directory

Edit the line in Dockerfile replacing API_KEY_HERE with your API key.

```
sudo docker build -t aiscantech_demo .
sudo docker run --rm aiscantech_demo
```


