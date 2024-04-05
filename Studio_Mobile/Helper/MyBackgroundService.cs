using Microsoft.Extensions.Hosting;
using Soc_Management_Web.Models;
using Studio_Mobile.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Studio_Mobile.Helper
{
	public class MyBackgroundService : BackgroundService
	{
		
		public MyBackgroundService()
		{
			
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await SendTimelyNotificationAsync();
				await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Wait for 5 minutes
			}
		}

		public async Task SendTimelyNotificationAsync()
		{
			DbConnections _connection = new DbConnections();
SqlParameter[] sqlParameters = new SqlParameter[3];
			sqlParameters[0] = new SqlParameter("@Id", 1);
			sqlParameters[1] = new SqlParameter("@status", 0);
			sqlParameters[2] = new SqlParameter("@flag", 1);
			DataTable dtlogin = _connection.CallStoreProcedure("prc_mob_getPushnoification", sqlParameters);
			if (dtlogin.Rows.Count > 0)
			{
				for(int i=0;i< dtlogin.Rows.Count;i++)
				{
					await NotificationService.NotifyAsync(dtlogin.Rows[i]["DeviceId"].ToString(), dtlogin.Rows[i]["OrtEvnNm"].ToString(), dtlogin.Rows[i]["OrtVenNm"].ToString());
				}

			}
			
				
		}
	}
}
