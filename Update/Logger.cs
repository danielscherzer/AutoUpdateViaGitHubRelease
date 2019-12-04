using System;
using System.IO;

namespace Update
{
	class Logger
	{
		private string _logFileName;

		public Logger(string logFileName)
		{
			LogFileName = logFileName;
		}

		public string LogFileName
		{
			get => _logFileName;
			set
			{
				_logFileName = value;
				Log($"Logging to {LogFileName}");
			}
		}

		public void Log(string message)
		{
			var time = DateTime.Now.ToString();
			var entry = $"{time}: {message}{Environment.NewLine}";
			File.AppendAllText(LogFileName, entry);
			Console.WriteLine(entry);
		}
	}
}
