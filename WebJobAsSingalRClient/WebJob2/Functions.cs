using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace WebJob2
{
	public class UserDetails
	{
		public int OrgId { get; set; }
		public int UserId { get; set; }
	}


	public class Functions
	{
		[NoAutomaticTrigger]
		public static void ManualTrigger(TextWriter log, int value, [Queue("queue")] out string message)
		{
			var c = new Functions();

			c.InvokeHub().Wait();

			message = "hi.";
			Console.Write(message);

			log.WriteLine("Following message will be written on the Queue={0}", message);
		}


		public async Task InvokeHub()
		{
			var complexObject = new List<UserDetails>
           {
			new UserDetails{ UserId = 1143, OrgId= 4356 },
			new UserDetails{ UserId = 1144, OrgId= 4356 }
           };

			//const int data = 1143;
			//new JavaScriptSerializer().Serialize(complexObject);

			var data = JsonConvert.SerializeObject(complexObject);

			Console.WriteLine(data);
			//Thread.Sleep(100000);

			var hub = new HubConnection("http://localhost:53561/");

			var proxy = hub.CreateHubProxy("MyHub");

			await hub.Start();
			await proxy.Invoke("SendMessage", data);

			hub.Stop();

		}
	}
}
