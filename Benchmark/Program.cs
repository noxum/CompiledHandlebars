using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace CompiledHandlebars.Benchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      var benchCase = Benchmarker.CreateFullBenchmark();
      if (args[0].Equals("-s") && args.Length == 3)
      {//if save-flag is set -> write benchmark result to blobstorage as json
        var commitHash = args[1];
        var cloudStorageConnectionString = args[2];
        var storageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference("results");      
        var blockBlob = container.GetBlockBlobReference($"{commitHash}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
        var json = JsonConvert.SerializeObject(benchCase, Formatting.Indented);
        blockBlob.UploadText(json);
      }      
    }       


   
  }
}
