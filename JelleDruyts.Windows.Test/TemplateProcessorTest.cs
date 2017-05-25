using System;
using System.Collections.Generic;
using JelleDruyts.Windows.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class TemplateProcessorTest
    {
        #region TestContext

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConstructorTemplateNull()
        {
            TemplateProcessor processor = new TemplateProcessor(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTokenFormatExpressionNull()
        {
            TemplateProcessor processor = new TemplateProcessor("");
            processor.TokenFormatExpression = null;
        }

        [TestMethod]
        public void TestConstructor()
        {
            TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName) $(LastName), you were born on $(BirthDate)");
            Assert.AreEqual<string>("Hello $(FirstName) $(LastName), you were born on $(BirthDate)", processor.Template);
        }

        [TestMethod]
        public void TestProcess()
        {
            TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName) $(LastName), you were born on $(BirthDate)");
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("FirstName", "Jeff");
            tokenValues.Add("LastName", "Buckley");
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello Jeff Buckley, you were born on 17/11/1966 0:00:00", processed);
        }

        [TestMethod]
        public void TestProcessDoesntTouchUnusedTokens()
        {
            TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName) $(LastName), you were born on $(BirthDate)");
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("FirstName", "Jeff");
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello Jeff $(LastName), you were born on $(BirthDate)", processed);
        }

        [TestMethod]
        public void TestProcessWithFormat()
        {
            TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName:Dummy) $(LastName:), you were born on $(BirthDate:yyyyMMdd)");
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("FirstName", "Jeff");
            tokenValues.Add("LastName", "Buckley");
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello Jeff Buckley, you were born on 19661117", processed);
        }

        [TestMethod]
        public void TestProcessWithCustomFormat()
        {
            string customFormat = @"<(?<Name>.*?)(\s+format=""(?<Format>.*?)"")?\s*/>";
            TemplateProcessor processor = new TemplateProcessor("You were born on <BirthDate format=\"yyyyMMdd\"/>", customFormat);
            Assert.AreEqual<string>(customFormat, processor.TokenFormatExpression);
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("You were born on 19661117", processed);
        }

        [TestMethod]
        public void TestProcessResetWithCustomFormat()
        {
            TemplateProcessor processor = new TemplateProcessor("Hello $(FirstName) <BirthDate format=\"yyyyMMdd\"/>");
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("FirstName", "Jeff");
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello Jeff <BirthDate format=\"yyyyMMdd\"/>", processed);

            string customFormat = @"<(?<Name>.*?)(\s+format=""(?<Format>.*?)"")?\s*/>";
            processor.TokenFormatExpression = customFormat;
            processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello $(FirstName) 19661117", processed);
        }

        [TestMethod]
        public void TestStaticProcess()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = TemplateProcessor.Process("You were born on $(BirthDate:yyyyMMdd)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("You were born on 19661117", processed);
        }

        [TestMethod]
        public void TestStaticProcessWithCustomFormat()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("BirthDate", new DateTime(1966, 11, 17));
            string processed = TemplateProcessor.Process("You were born on <BirthDate format=\"yyyyMMdd\"/>", tokenValues, @"<(?<Name>.*?)(\s+format=""(?<Format>.*?)"")?\s*/>");
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("You were born on 19661117", processed);
        }

        [TestMethod]
        public void TestTokenNull()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Name", null);
            TemplateProcessor processor = new TemplateProcessor("Hello $(Name)!");
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello !", processed);
        }

        [TestMethod]
        public void TestTokenNullWithNullText()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Name", null);
            TemplateProcessor processor = new TemplateProcessor("Hello $(Name)!");
            processor.TokenValueNullText = "World";
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hello World!", processed);
        }

        [TestMethod]
        public void TestArray()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Names", new string[] { "Jeff", "John", "Jane" });
            TemplateProcessor processor = new TemplateProcessor("Hi there $(Names)");
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hi there Jeff, John, Jane", processed);
        }

        [TestMethod]
        public void TestArrayWithSeparator()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Names", new string[] { "Jeff", "John", "Jane" });
            TemplateProcessor processor = new TemplateProcessor("Hi there $(Names)");
            processor.ArraySeparator = "/";
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hi there Jeff/John/Jane", processed);
        }

        [TestMethod]
        public void TestArrayWithSeparatorNull()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Names", new string[] { "Jeff", "John", "Jane" });
            TemplateProcessor processor = new TemplateProcessor("Hi there $(Names)");
            processor.ArraySeparator = null;
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hi there JeffJohnJane", processed);
        }

        [TestMethod]
        public void TestArrayWithFormat()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("BirthDates", new DateTime[] { new DateTime(2000, 1, 1), new DateTime(2005, 2, 2) });
            TemplateProcessor processor = new TemplateProcessor("Hi there $(BirthDates:yyyyMMdd)");
            string processed = processor.Process(tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Hi there 20000101, 20050202", processed);
        }

        [TestMethod]
        public void TestStringFormattingTrim()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Text", "       Hi There    ");
            string processed = TemplateProcessor.Process("Here's the text: $(Text:Trim)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the text: Hi There", processed);
        }

        [TestMethod]
        public void TestStringFormattingLowerCase()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Text", "Hi There");
            string processed = TemplateProcessor.Process("Here's the text: $(Text:LowerCase)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the text: hi there", processed);
        }

        [TestMethod]
        public void TestStringFormattingUpperCase()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Text", "Hi There");
            string processed = TemplateProcessor.Process("Here's the text: $(Text:UpperCase)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the text: HI THERE", processed);
        }

        [TestMethod]
        public void TestStringFormattingTitleCase()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Text", "hi there");
            string processed = TemplateProcessor.Process("Here's the text: $(Text:TitleCase)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the text: Hi There", processed);
        }

        [TestMethod]
        public void TestStringFormattingHtmlEncoded()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("HtmlText", "<body>Hi there</body>");
            string processed = TemplateProcessor.Process("Here's the html: $(HtmlText:HtmlEncoded)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the html: &lt;body&gt;Hi there&lt;/body&gt;", processed);
        }

        [TestMethod]
        public void TestStringFormattingHtmlDecoded()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("HtmlText", "&lt;body&gt;Hi there&lt;/body&gt;");
            string processed = TemplateProcessor.Process("Here's the html: $(HtmlText:HtmlDecoded)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the html: <body>Hi there</body>", processed);
        }

        [TestMethod]
        public void TestStringFormattingIgnoresFormatCasing()
        {
            Dictionary<string, object> tokenValues = new Dictionary<string, object>();
            tokenValues.Add("Text", "Hi There");
            string processed = TemplateProcessor.Process("Here's the text: $(Text:uppercase)", tokenValues);
            Assert.IsNotNull(processed);
            Assert.AreEqual<string>("Here's the text: HI THERE", processed);
        }
    }
}