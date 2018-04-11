using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using System.IO;
using RestSharp;

using Newtonsoft.Json;

//Demo of AI Scan Tech API calls from dotnet
//The API works whereby you upload an image of a point of sale receipt (see an example under receipt_images/receipt_test.jpg)
//in this repository) and are returned a token, you short poll the server using the token to listen for the results

//You will need to sign up for an API key at https://tabscanner.com
//Note: when testing the API the server will need time to launch for the first call, subsequent calls should complete in ~3 seconds


namespace aiscantech_demo
{
    class Program
    {

        private static string api = "https://api.tabscanner.com";
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> UploadImage(string fileName, string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddFile("file", fileName, "image/jpeg");

            var result = await client.ExecuteTaskAsync<ProcessResult>(request);

            return result.Data.token;

        }

        private static async Task<ReceiptResult> ProcessResult(string url)
        {
            //convert to dotnet object
            var serializer = new DataContractJsonSerializer(typeof(ReceiptResult));
            var streamTask = client.GetStreamAsync(url);
            var result = serializer.ReadObject(await streamTask) as ReceiptResult;

            return result;
        }

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("API key must be provided as the first argument to the command.");
                return;
            }

            if (args.Length == 1)
            {
                if (args[0] == "API_KEY_HERE")
                {
                    Console.WriteLine("API key must be provided as the first argument to the command in your Dockerfile.");
                    return;
                }
                
            }

            //build the url to make the request

            string key = args[0];

            string processUrl = String.Format("{0}/{1}/process", api, key);

            Console.WriteLine("Uploading receipt...");

            //the file name of the receipt to upload
            string fileName = "receipt_images/receipt_test.jpg";

            //upload the image and receive the token to use when listenign for the result
            var token = UploadImage(fileName, processUrl).Result;

            //build the url to listen for the resuly
            string resultUrl = String.Format("{0}/{1}/result/{2}", api, key, token);


            //check for a result every 5 seconds
            //change this according to your preference
            ReceiptResult result = ProcessResult(resultUrl).Result;
            int pollLimit = 20;
            int p = 0;

            while (result.status == "pending" && p < pollLimit)
            {
                result = ProcessResult(resultUrl).Result;

                //if the call returns a done status exit the listening process
                if (result.status == "done")
                {
                    break;
                }
                System.Threading.Thread.Sleep(5000);
                p++;
                Console.WriteLine("Listening for result...");
            }

            if (result.status != "done")
            {
                //something went wrong, please contact us!
                Console.WriteLine(result.status);
                Console.WriteLine(result.message);
                Console.WriteLine("Polling for result failed.");
            }
            else
            {
                //the result is available deserialised as a dotnet class
                Console.WriteLine("Result available.");

                Receipt receipt = result.result;
                //for a full list of properties see class definition in Receipt.cs
                Console.WriteLine(receipt.establishment);
                Console.WriteLine(receipt.total);
                Console.WriteLine(receipt.cash);
                Console.WriteLine(receipt.change);
                Console.WriteLine(receipt.tax);

                foreach (LineItem li in receipt.lineItems)
                {
                    Console.WriteLine(li.desc);
                    Console.WriteLine(li.lineTotal);
                    Console.WriteLine("---------------");
                }

                //reserialize as json and save to a file
                var json = JsonConvert.SerializeObject(result.result);

                var file = File.CreateText("results/result.json");
                file.Write(json);
                file.Close();

            }

        }
    }
}
