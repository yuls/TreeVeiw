using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeVeiw.Model;

namespace TreeVeiw.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TreeViewController : ControllerBase
    {
        private readonly ILogger<TreeViewController> _logger;

        public TreeViewController(ILogger<TreeViewController> logger)
        {
            _logger = logger;
        }

        [HttpGet("rootnodes/{sortby?}")]
        public IEnumerable<TreeViewNode> RootNodes(int sortby = 0)
        {
            Data.TreeViewDbData data = new Data.TreeViewDbData();
            var nodes = data.GetTreeViewNode();

            return nodes;
        }

        [HttpGet("childnodes/{parentId}/{sortby?}")]
        public IEnumerable<TreeViewNode> ChildNodes(int parentId, int sortby = 0)
        {
            Data.TreeViewDbData data = new Data.TreeViewDbData();
            var nodes = data.GetChildNodes(parentId, sortby);

            return nodes;
        }

        [HttpPost("updatenode/{childId}/{parentId}")]
        public void UpdateParentNode(int childId, int parentId)
        {
            Data.TreeViewDbData data = new Data.TreeViewDbData();
            data.UpdateNodeParentId(childId, parentId);
        }
    }
}
