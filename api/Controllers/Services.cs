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
    public class Services : Common
    {
        [HttpPost("create/{Identifikation}")]
        public void Create(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"chmod +x {Identifikation}\"");
        }

        [HttpPost("reload")]
        public void Reload()
        {
            General.Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
        }

        [HttpPost("enable/{Identifikation}")]
        public void Enable(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user enable --now {Identifikation}\"");
        }

        [HttpPost("disable/{Identifikation}")]
        public void Disable(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user disable --now {Identifikation}\"");
        }

        [HttpPost("start/{Identifikation}")]
        public void Start(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user start --now {Identifikation}\"");
        }

        [HttpPost("stop/{Identifikation}")]
        public void Stop(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user stop --now {Identifikation}\"");
        }

        [HttpPost("restart/{Identifikation}")]
        public void Restart(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user restart --now {Identifikation}\"");
        }
    }
}
