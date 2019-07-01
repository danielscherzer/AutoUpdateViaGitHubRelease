using System;
using System.IO;
using System.Reflection;
using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			//var update = new Update("danielScherzer", "BatchExecute", Assembly.GetExecutingAssembly(), Path.GetTempPath());
			//update.PropertyChanged += (s, a) => Available = update.Available;
		}
	}
}
