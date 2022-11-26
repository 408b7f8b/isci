using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;

namespace isci.Anwendungen
{
    public class Ereignis : Header {
        public string Ausloeser;
        public List<string> Elemente;
    }
}