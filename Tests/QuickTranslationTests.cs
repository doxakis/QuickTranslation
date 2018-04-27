using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickTranslationPack;
using System;
using System.Globalization;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class QuickTranslationTests
    {
        [TestMethod]
        public void TestBasicUsage()
        {
			var text = QuickTranslation.Prepare(@"
				# Comment.
				010: english1
				- fr: french1
				- es: spanish1
				
				020: english2
				030: english3
			");

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Assert.AreEqual("english1", text["010"]);
			Assert.AreEqual("english2", text["020"]);
			Assert.AreEqual("english3", text["030"]);

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");
			Assert.AreEqual("french1", text["010"]);
			Assert.AreEqual("english2", text["020"]);
			Assert.AreEqual("english3", text["030"]);
		}

		[TestMethod]
		public void TestDuplicate()
		{
			// Duplicates are ignored.
			// We use the first one we found.

			var text = QuickTranslation.Prepare(@"
				010: english1
				- fr: french2
				- fr: french3
				
				010: english4
				- fr: french5
				- fr: french6
			");

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Assert.AreEqual("english1", text["010"]);

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");
			Assert.AreEqual("french2", text["010"]);
		}

		[TestMethod]
		public void TestNoSpaces()
		{
			var text = QuickTranslation.Prepare(
				"010:english1" + Environment.NewLine +
				"-fr:french2");

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Assert.AreEqual("english1", text["010"]);

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");
			Assert.AreEqual("french2", text["010"]);
		}

		[TestMethod]
		public void TestMissingLang()
		{
			var text = QuickTranslation.Prepare(
				"010:english1" + Environment.NewLine +
				"-:french2");

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Assert.AreEqual("english1", text["010"]);

			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-CA");
			Assert.AreEqual("english1", text["010"]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestMissingCode()
		{
			var text = QuickTranslation.Prepare("");
			
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			var translation = text["010"];
		}
	}
}
