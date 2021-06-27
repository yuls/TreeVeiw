using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeVeiw.Model
{
    public class TreeViewNode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte Type { get; set; }

        public double? Size { get; set; }

        public int? ParentNode { get; set; }
    }
}
