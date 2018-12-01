using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureAPI.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlanCostShares
    {
        public int deductible { get; set; }
        public string _org { get; set; }
        public int copay { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
    }

    public partial class LinkedService
    {
        public string _org { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
        public string name { get; set; }
    }

    public class PlanserviceCostShares
    {
        public int deductible { get; set; }
        public string _org { get; set; }
        public int copay { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
    }

    public partial class LinkedPlanService
    {
        public LinkedService linkedService { get; set; }
        public PlanserviceCostShares planserviceCostShares { get; set; }
        public string _org { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
    }

    public class RootObject
    {
        public PlanCostShares planCostShares { get; set; }
        public List<LinkedPlanService> linkedPlanServices { get; set; }
        public string _org { get; set; }
        public string objectId { get; set; }
        public string objectType { get; set; }
        public string planType { get; set; }
        public string creationDate { get; set; }
    }

}

