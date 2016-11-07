using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace WebJobAsSingalRClient
{
	public class UserDetails
	{
		public int OrgId { get; set; }
		public int UserId { get; set; }
		public List<string> ConnectionIds { get; set; }
	}

	public class MyHub : Hub
	{
		private readonly static ConnectionMapping<string> Connections = new ConnectionMapping<string>();

		public async Task SendMessage(string message)
		{
			var x = message;
			
			var receivedData = (List<UserDetails>)JsonConvert.DeserializeObject(message, typeof(List<UserDetails>));

			#region

			foreach (var eachItem in receivedData)
			{
				Console.WriteLine(eachItem.UserId);

				var onlineUserId = eachItem.UserId;
				var onlineUserOrgId = eachItem.OrgId;

				string jobid = GetUserId();

				if (jobid == null)
				{
					jobid = onlineUserOrgId + "-" + onlineUserId;


					foreach (var connectionId in Connections.GetConnections(jobid))
					{
						//var complexObject = new List<FakeNotificationModel>
						//{
						// new FakeNotificationModel(){ Name = "MyName", Age= 100 },
						// new FakeNotificationModel(){ Name = "YourName", Age= 435 }
						//};
						////var json = new JavaScriptSerializer().Serialize(complexObject);

						var json = onlineUserId;
						await Clients.Client(connectionId).send(json);
					}
				}
				else
				{
					foreach (var connectionId in Connections.GetConnections(jobid))
					{
						await Clients.Client(connectionId).send(jobid);
					}
				}
			}
			#endregion
		}


		public override Task OnConnected()
		{
			string jobid = GetUserId();

			if (jobid != null)
			{
				Connections.Add(jobid, Context.ConnectionId);
			}
			return base.OnConnected();
		}


		public override Task OnReconnected()
		{
			string jobId = GetUserId();

			if (!Connections.GetConnections(jobId).Contains(Context.ConnectionId))
			{
				Connections.Add(jobId, Context.ConnectionId);
			}

			return base.OnReconnected();
		}


		public override Task OnDisconnected(bool stopCalled)
		{
			string jobid = GetUserId();

			Connections.Remove(jobid, Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		private string GetUserId()
		{
			string userId = Context.QueryString["userid"];
			return userId;
		}
	}
}