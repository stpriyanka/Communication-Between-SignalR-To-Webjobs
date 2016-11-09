using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace WebJobAsSingalRClient
{
	public class UserDetails
	{
		public int UserId { get; set; }
	}


	public class MyHub : Hub
	{
		private readonly static ConnectionMapping<string> Connections = new ConnectionMapping<string>();

		public async Task SendMessage(string message)
		{
			var receivedData = (List<UserDetails>)JsonConvert.DeserializeObject(message, typeof(List<UserDetails>));

			foreach (var eachItem in receivedData)
			{
				var onlineUserId = eachItem.UserId.ToString();

				string uniqueData = GetQueryString();

				if (uniqueData != null)
				{
					if (uniqueData != onlineUserId) continue;
					foreach (var connectionId in Connections.GetConnections(uniqueData))
					{
						await Clients.Client(connectionId).send(uniqueData + " hello !");
					}
				}
			}
		}


		public override Task OnConnected()
		{
			string jobid = GetQueryString();

			if (jobid != null)
			{
				Connections.Add(jobid, Context.ConnectionId);
			}
			return base.OnConnected();
		}


		public override Task OnReconnected()
		{
			string jobId = GetQueryString();

			if (!Connections.GetConnections(jobId).Contains(Context.ConnectionId))
			{
				Connections.Add(jobId, Context.ConnectionId);
			}

			return base.OnReconnected();
		}


		public override Task OnDisconnected(bool stopCalled)
		{
			string jobid = GetQueryString();

			Connections.Remove(jobid, Context.ConnectionId);

			return base.OnDisconnected(stopCalled);
		}

		private string GetQueryString()
		{
			string userId = Context.QueryString["userid"];
			return userId;
		}
	}
}