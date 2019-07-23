using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Utils
{
    public enum SemanticType
    {
        Border = 0,
        Text = 1,
        Background = 2
    }

    [DataContract]
    public class SemanticRegion
    {
        [DataMember]
        public BoundingBox Box { get; set; }
        [DataMember]
        public SemanticType SemanticType { get; set; }
    }

    [DataContract]
    public class SemanticRegions
    {
        [DataMember]
        public List<SemanticRegion> Regions { get; set; }
    }
}
