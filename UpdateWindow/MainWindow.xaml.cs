using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UpdateWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string logFileName;

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			logFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "update.log");
			Log($"Logging to {logFileName}");
			try
			{
				var args = Environment.GetCommandLineArgs();
				if(3 != args.Length)
				{
					Log($"Usage: {nameof(UpdateWindow)} <{nameof(Options.UpdateDataArchive)}> <{nameof(Options.ApplicationDir)}>");
					return;
				}
				var options = new Options( args[1], args[2] );
				await Task.Run(() => Update(options));
				Thread.Sleep(3000);
				Application.Current.Shutdown();
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}

		private void Log(string message)
		{
			var time = DateTime.Now.ToLongTimeString();
			var entry = $"{time}: {message}{Environment.NewLine}";
			Dispatcher.Invoke(() =>
			{
				File.AppendAllText(logFileName, entry);
				log.AppendText(entry);
				log.ScrollToEnd();
			});
		}

		private void Update(Options options)
		{
			if (!Directory.Exists(options.ApplicationDir)) throw new DirectoryNotFoundException(options.ApplicationDir);
			logFileName = Path.Combine(options.ApplicationDir, Path.GetFileName(logFileName));
			Log($"Logging to {logFileName}");
			if (!File.Exists(options.UpdateDataArchive)) throw new FileNotFoundException(options.UpdateDataArchive);

			using (var file = File.OpenRead(options.UpdateDataArchive))
			{
				try
				{
					using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
					{
						foreach (var entry in zip.Entries)
						{
							var destinationFile = Path.Combine(options.ApplicationDir, entry.FullName);
							TryDeleteWait(destinationFile);
							Log($"Creating new {destinationFile}");
							try
							{
								entry.ExtractToFile(destinationFile);
							}
							catch
							{
								//file still in use, no permission -> stop
								Log($"Error creating new {destinationFile}");
								return;
							}
						}
					}
				}
				catch
				{
					var destinationFile = Path.Combine(options.ApplicationDir, Path.GetFileName(options.UpdateDataArchive));
					TryDeleteWait(destinationFile);
					Log($"Creating new {destinationFile}");
					try
					{
						using (var destination = File.Create(destinationFile))
						{
							file.CopyTo(destination);
						}
					}
					catch
					{
						//file still in use, no permission -> stop
						Log($"Error creating new {destinationFile}");
						return;
					}
				}
			}
			Log($"Update Finished");
		}

		private bool TryDeleteWait(string destinationFile, int tries = 10, int waitTimeMsec = 1000)
		{
			for (var i = 0; i < tries; ++i)
			{
				Log($"Try {i} delete {destinationFile}");
				try
				{
					// try to delete
					File.Delete(destinationFile);
					// successful, so we can write new version
					return true;
				}
				catch
				{
					// unsuccessful -> wait before next try
					Thread.Sleep(waitTimeMsec);
				}
			}
			return false;
		}

		private static void Run(string executablePath, string parameters)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = executablePath,
					Arguments = parameters,
					WorkingDirectory = Path.GetDirectoryName(executablePath),
					RedirectStandardOutput = false,
					RedirectStandardError = false,
				}
			};
			process.Start();
		}
	}
}
