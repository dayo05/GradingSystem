using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace IroojGradingSystem
{
    public class Grad : Base
    {
        public Grad(StreamWriter stream, string language, string source) : base(stream)
        {
            var xml = XDocument.Load("language.xml").Root.Element(language);
            if (xml == null) throw new NullReferenceException("Language not found");
            base.CompileString = new List<string>((from x in xml.Elements("compile_string") select x.Value).ToList());
            base.RunScript = xml.Element("execute_string")?.Value;
            var sourceCode = new StreamWriter("grad/" + xml.Element("source_file")?.Value!);
            sourceCode.WriteLine(source);
            sourceCode.Close();
        }
    }
}