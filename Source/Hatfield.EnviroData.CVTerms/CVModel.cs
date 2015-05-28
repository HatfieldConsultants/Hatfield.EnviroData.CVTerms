using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.CVUpdater
{
    public class CVModel
    {
        public string Term { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Category { get; set; }
        public string SourceVocabularyURI { get; set; }
    }
}
