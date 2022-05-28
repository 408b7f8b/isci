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
    public class Applications : Common
    {
        public Applications()
        {
            target_path = "applications";
        }
        
        [HttpGet]
        new public ActionResult<string> Get()
        {
            var list = General.FoldersInPath(target_path);

            return list;
        }
    }
}
