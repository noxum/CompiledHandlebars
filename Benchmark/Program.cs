using CompiledHandlebars.Benchmark.MeasurementModels;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CompiledHandlebars.Benchmark
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Started Benchmark...");
      var benchCase = Benchmarker.CreateFullBenchmark();
      PrintSummary(benchCase.Summary.Items.Select(x => x.Value).ToList());
      if (args.Length == 3 && args[0].Equals("-s"))
      {//if save-flag is set -> write benchmark result to blobstorage as json
        var commitHash = args[1];
        benchCase.CommitHash = commitHash;
        var cloudStorageConnectionString = args[2];
        var storageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference("results");      
        var blockBlob = container.GetBlockBlobReference($"{commitHash}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json");
        var json = JsonConvert.SerializeObject(benchCase, Formatting.Indented);
        blockBlob.UploadText(json);
      }      
    }       


    static void PrintSummary(List<BenchmarkSummary.BenchmarkSummaryItem> items)
    {
      Console.WriteLine($"{"Name",15}{"Average Throughput", 22}{"Standard Deviation",22}{"Sample Size",15}");
      foreach (var item in items)
        Console.WriteLine($"{item.Name,15}{item.AverageThroughput,22}{item.StandardDeviation,22}{item.SampleSize,15}");
    }

   
  }
}
