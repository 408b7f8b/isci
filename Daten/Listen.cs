using System;
using System.Linq;

namespace isci.Daten
{
    public class ListeDateneintraege : System.Collections.Generic.List<Dateneintrag>
    {
        public new void Add(Dateneintrag item)
        {
            if (this.Where(entry => entry.Identifikation == item.Identifikation).Count() > 0) return;
            /*foreach (var entry in this)
            {
                if (entry.Identifikation == item.Identifikation)
                {
                    return;
                }
            }*/
            base.Add(item);
        }

        public void AddRange(ListeDateneintraege items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
    }

    public class KarteDateneintraege : System.Collections.Generic.Dictionary<string, Dateneintrag>
    {
        public string Serialisieren()
        {
            var ret = new Newtonsoft.Json.Linq.JArray();

            foreach (var eintrag in this)
            {
                ret.Add(eintrag.Value.DateneintragAlsJToken());
            }

            return ret.ToString(Newtonsoft.Json.Formatting.None);
        }
    }

    public class VerweislisteDateneintraege : System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>
    {
        public void Add(VerweislisteDateneintraege item)
        {
            foreach (var entry in item)
            {
                this.Add(entry.Key, entry.Value);
            }        
        }

        public new void Add(string item1, System.Collections.Generic.List<string> item2)
        {
            if (this.ContainsKey(item1))
            {
                foreach (var entry in item2)
                {
                    if (!this[item1].Contains(entry))
                    {
                        this[item1].Add(entry);
                    }
                }
            } else {
                base.Add(item1, item2);
            }            
        }

        public void Add(string item1, string item2)
        {
            if (this.ContainsKey(item1))
            {
                if (!this[item1].Contains(item2))
                {
                    this[item1].Add(item2);
                }
            } else {
                base.Add(item1, new System.Collections.Generic.List<string>(){item2});
            }
        }
    }

    public class VerweislisteDateneintraegeAktiv : System.Collections.Generic.Dictionary<Dateneintrag, ListeDateneintraege>
    {
        public void Add(VerweislisteDateneintraegeAktiv item)
        {
            foreach (var entry in item)
            {
                this.Add(entry.Key, entry.Value);
            }        
        }

        public new void Add(Dateneintrag item1, ListeDateneintraege item2)
        {
            if (this.ContainsKey(item1))
            {
                foreach (var entry in item2)
                {
                    if (!this[item1].Contains(entry))
                    {
                        this[item1].Add(entry);
                    }
                }
            } else {
                base.Add(item1, item2);
            }            
        }

        public void Add(Dateneintrag item1, Dateneintrag item2)
        {
            if (this.ContainsKey(item1))
            {
                if (!this[item1].Contains(item2))
                {
                    this[item1].Add(item2);
                }
            } else {
                base.Add(item1, new ListeDateneintraege(){item2});
            }
        }

        public void Aktualisieren(Dateneintrag item1)
        {
            if (this.ContainsKey(item1))
            {
                foreach (var subItem in this[item1])
                {
                    subItem.Wert = item1.Wert;
                }
            }
        }
    }
}