using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace grafikaPZ3.Model
{
    public class XmlEntities
    {
        public XmlEntities()
        {
            Lines = new List<LineEntity>();
            Nodes = new List<NodeEntity>();
            Switches = new List<SwitchEntity>();
            Substations = new List<SubstationEntity>();
        }

        public List<SubstationEntity> Substations { get; set; }
        public List<NodeEntity> Nodes { get; set; }
        public List<SwitchEntity> Switches { get; set; }
        public List<LineEntity> Lines { get; set; }
    }
}
