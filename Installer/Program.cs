using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Installer
{
	class Program
	{
		static void Main(string[] args)
		{
			var logger = new Logger(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".log"));
			var install = new Install(logger);
			try
			{
				if (2 != args.Length)
				{
					logger.Log($"Usage: Installer <UpdateDataArchive> <ApplicationDir>");
					return;
				}
				var updateDataArchive = args[0];
				var applicationDir = args[1];
				logger.Log($"Parameter are {nameof(updateDataArchive)}='{updateDataArchive}' and {nameof(applicationDir)}='{applicationDir}'");
				install.Execute(applicationDir, updateDataArchive);
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
