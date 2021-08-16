using Jot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient.Services
{
	// Expose services as static class to keep the example simple 
	public static class Services
	{
		// expose the tracker instance
		public static Tracker Tracker = new Tracker();

		static Services()
		{
			// tell Jot how to track Window objects
		}
	}

}
