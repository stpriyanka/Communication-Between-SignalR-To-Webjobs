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
			var functions = new Functions();
			functions.InvokeHub().Wait();

			message = "Pushed data to server.";
			Console.Write(message);

			log.WriteLine("Following message will be written on the Queue={0}", message);
		}


		public async Task InvokeHub()
		{
			var userdetails = new List<UserDetails>
           {
			new UserDetails{ UserId = 56-43 },
			new UserDetails{ UserId = 55-44 }
           };

			var data = JsonConvert.SerializeObject(userdetails);

			Console.WriteLine(data);

			var hub = new HubConnection("http://localhost:53561/");
			var proxy = hub.CreateHubProxy("MyHub");
			await hub.Start();
			await proxy.Invoke("SendMessage", data);
			hub.Stop();
		}
	}
}
