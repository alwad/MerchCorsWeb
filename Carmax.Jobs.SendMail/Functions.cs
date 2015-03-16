using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carmax.Jobs.SendMail
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called sendmail.
        public static void ProcessQueueMessage([Microsoft.Azure.WebJobs.ServiceBusTrigger("sendmail")] BrokeredMessage message, TextWriter log)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            log.WriteLine("{0}; Json: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, json);
            Console.WriteLine("THREAD [{0}] \r\n\r\n Json: {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, json);
            System.Threading.Thread.Sleep(1000);
            message.Complete();
        }
    }
}
