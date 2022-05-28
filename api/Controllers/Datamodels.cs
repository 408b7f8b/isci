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
    public class Datamodels : Common
    {
        public Datamodels()
        {
            target_path = "datamodels";
        }
    }
}
