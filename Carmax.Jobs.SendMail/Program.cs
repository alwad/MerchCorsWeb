using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.IO;

namespace Carmax.Jobs.SendMail
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {

            JobHostConfiguration config = new JobHostConfiguration();

            config.Queues.BatchSize = 16;
            config.Queues.MaxDequeueCount = 4;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(240);
        

            //while (Console.ReadKey().KeyChar != 'q')
            //{
            //    EnqeueMessage();
            //    WriteEmailMessage();
            //    Console.WriteLine("message queued - 'q' to exit");
            //}

            var host = new JobHost();
            host.RunAndBlock();
        }

        private static void EnqeueMessage()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=merchjobs;AccountKey=Y3mhdW77BfQxgdTswDNGR7s4Z5URldp8DPwmCBBUoXU4Avna+12dkDP7xzzeIMaB2GNyKG2Zj+ldR2F4T09HsQ==");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("sendmail");
            queue.CreateIfNotExists();
            CloudQueueMessage msg = new CloudQueueMessage(
                "test"
              );
            queue.AddMessage(msg);

            
        }

        private static void WriteEmailMessage()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=merchjobs;AccountKey=Y3mhdW77BfQxgdTswDNGR7s4Z5URldp8DPwmCBBUoXU4Avna+12dkDP7xzzeIMaB2GNyKG2Zj+ldR2F4T09HsQ==");
            var client = storageAccount.CreateCloudBlobClient();
            var blobs = client.GetContainerReference("email");
            blobs.CreateIfNotExists();
            
            var blob = blobs.GetBlockBlobReference(Guid.NewGuid().ToString());
            
            byte[] byteArray = Encoding.UTF8.GetBytes("test test 123");
            blob.UploadFromStream(new MemoryStream(byteArray));


        }

    }
}
