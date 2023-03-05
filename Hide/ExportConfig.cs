using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hide
{
    public class ExportConfig
    {
        public string OutputPath { get; set; }

        public int LevelOfDetail { get; set; }

        public List<int> Filter { get; set; }
    }
}
