using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

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

            config.ServiceBusConnectionString = "Endpoint=sb://codestrike.servicebus.windows.net/;SharedAccessKeyName=submitter;SharedAccessKey=tFXZ7Ms+Y3JKE+MTt/5p8065au4y0U1eQ20V3quQVU0=";

            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
