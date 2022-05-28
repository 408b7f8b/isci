using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Environment : Common
    {
        [HttpPost("add/{Identifikation}")]
        public void Add(string Identifikation, [FromBody]string value)
        {
            var neu = new List<string>();

            try {
                var inh = System.IO.File.ReadAllLines("~/.bashrc");

                int i = 0;
                for (; i < inh.Length; ++i) {
                    if (inh[i].Contains(Identifikation))
                    {
                        inh[i] = "export " + Identifikation + "=" + value;
                        i = inh.Length+1;
                    }
                }
                if (i != inh.Length+1)
                {
                    neu.Add("export " + Identifikation + "=" + value);
                }

                System.IO.File.WriteAllLines("~/.bashrc", inh);
                if (neu.Count > 0) System.IO.File.AppendAllLines("~/.bashrc", neu);
            } catch {

            }
        }

        [HttpPost("change/{Identifikation}")]
        public void Change(string Identifikation, [FromBody]string value)
        {
            this.Add(Identifikation, value);
        }

        [HttpPost("remove/{Identifikation}")]
        public void Remove(string Identifikation)
        {
            var neu = new List<string>();

            try {
                var inh = System.IO.File.ReadAllLines("~/.bashrc");

                int i = 0;
                for (; i < inh.Length; ++i) {
                    if (inh[i].Contains("export " + Identifikation))
                    {

                    } else {
                        neu.Add(inh[i]);
                    }
                }
                if (inh.Length != neu.Count)
                {
                    System.IO.File.WriteAllLines("~/.bashrc", inh);
                }
            } catch {

            }
        }
    }
}













            