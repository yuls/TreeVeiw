using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace TreeVeiw.Data
{
    public class TreeViewDbData
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=C:\\USERS\\YULIYA\\SOURCE\\REPOS\\TREEVEIW\\TREEVEIW\\DB\\TREEVIEW.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public List<Model.TreeViewNode> GetTreeViewNode(int sortBy = 0)
        {
            List<Model.TreeViewNode> nodes = new List<Model.TreeViewNode>();

            string orderByColumn = sortBy == 0 ? "n.name" : "s.sizewithchildren Desc";

            string queryString = @"with all_sizes as
            (
                select n.id, n.size, n.id as rootid from nodes n
                union all
                select n.id, n.size, all_sizes.rootid from nodes n 
                       inner join all_sizes on n.parent = all_sizes.id
            )
            select n.id, n.name, n.type, s.sizewithchildren
            from nodes n 
                 inner join (select rootid, sum(size) as sizewithchildren from all_sizes group by rootid) as s
                 on n.id = s.rootid
            where n.parent is null
            order by n.type, " + orderByColumn ;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Model.TreeViewNode node = new Model.TreeViewNode();
                        node.Id = reader.GetInt32(0);
                        node.Name = reader.GetString(1);
                        node.Type = reader.GetByte(2);
                        node.Size = 0;
                        if (!reader.IsDBNull(3))
                            node.Size = reader.GetDouble(3);

                        node.ParentNode = null;
                        nodes.Add(node);
                    }

                    reader.Close();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            return nodes;
        }

        public List<Model.TreeViewNode> GetChildNodes(int parentNodeId, int sortBy = 0)
        {
            List<Model.TreeViewNode> nodes = new List<Model.TreeViewNode>();

            string orderByColumn = sortBy == 0 ? "n.name" : "s.sizewithchildren Desc";

            string queryString = @"with all_sizes as
            (
                select n.id, n.size, n.id as rootid from nodes n
                union all
                select n.id, n.size, all_sizes.rootid from nodes n 
                       inner join all_sizes on n.parent = all_sizes.id
            )
            select n.id, n.name, n.type, n.parent, s.sizewithchildren
            from nodes n 
                 inner join (select rootid, sum(size) as sizewithchildren from all_sizes group by rootid) as s
                 on n.id = s.rootid
            where n.parent = @parentid
            order by n.type, " + orderByColumn;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@parentid", parentNodeId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Model.TreeViewNode node = new Model.TreeViewNode();
                        node.Id = reader.GetInt32(0);
                        node.Name = reader.GetString(1);
                        node.Type = reader.GetByte(2);

                        node.ParentNode = null;
                        if (!reader.IsDBNull(3))
                            node.ParentNode = reader.GetInt32(3);

                        node.Size = null;
                        if (!reader.IsDBNull(4))
                            node.Size = reader.GetDouble(4);

                        

                        nodes.Add(node);
                    }

                    reader.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return nodes;
        }

        public void UpdateNodeParentId(int childNodeId, int parentNodeId)
        {
            
            string queryString = "UPDATE nodes SET parent = @parentId WHERE id = @childId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@parentId", parentNodeId);
                command.Parameters.AddWithValue("@childId", childNodeId);

                try
                {
                    connection.Open();
                    int rows = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}
