using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Update
{
	class Program
	{
		static void Main(string[] args)
		{
			var logger = new Logger(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".log"));
			var update = new Update(logger);
			try
			{
				if (2 != args.Length)
				{
					logger.Log($"Usage: Update <UpdateDataArchive> <ApplicationDir>");
					return;
				}
				update.Execute(applicationDir: args[0], updateDataArchive: args[1]);
			}
			catch (Exception ex)
			{
				logger.Log(ex.ToString());
			}
			for (int i = 5; i > 0; --i)
			{
				Console.WriteLine($"Closing in {i}seconds");
				Thread.Sleep(1000);
			}
		}
	}
}
