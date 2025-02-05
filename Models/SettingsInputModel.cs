using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NorbitsChallenge.Bll;

namespace NorbitsChallenge.Models
{
    public class SettingsInputModel
    {
        public Setting Setting { get; set; }
        public int CompanyId { get; set; }
    }
}
