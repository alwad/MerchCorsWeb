using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Carmax.Jobs.SendMail
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called sendmail.
        public static void ProcessQueueMessage([QueueTrigger("sendmail")] string message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }
}
