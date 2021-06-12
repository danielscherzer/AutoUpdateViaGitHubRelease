using System;
using System.IO;

namespace AutoUpdateViaGitHubRelease
{
	/// <summary>
	/// Basic log to file
	/// </summary>
	public class Logger
	{
		private string _logFileName;

		/// <summary>
		/// Create new instance of logger that writes to the given file
		/// </summary>
		/// <param name="logFileName">The path to the log file.</param>
		public Logger(string logFileName)
		{
			LogFileName = logFileName;
		}

		/// <summary>
		/// The path to the log file.
		/// </summary>
		public string LogFileName
		{
			get => _logFileName;
			set
			{
				_logFileName = value;
				Log($"Logging to {LogFileName}");
			}
		}

		/// <summary>
		/// Log the given message.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public void Log(string message)
		{
			var time = DateTime.Now.ToString();
			var entry = $"{time}: {message}{Environment.NewLine}";
			File.AppendAllText(LogFileName, entry);
			Console.WriteLine(entry);
		}
	}
}
