namespace UpdateWindow
{
	struct Options
	{
		public Options(string updateDataArchive, string applicationDir) : this()
		{
			UpdateDataArchive = updateDataArchive;
			ApplicationDir = applicationDir;
		}

		public string UpdateDataArchive { get; }

		public string ApplicationDir { get; }
	}
}
