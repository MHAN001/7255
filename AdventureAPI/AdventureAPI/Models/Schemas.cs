using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;

namespace AdventureAPI.Models
{
    public class Schemas
    {
        //public JSchema sampleSchema()
        //{
        //    JSchema res = new JSchema { Type = JSchemaType.Object,
        //        Properties = {
        //            { "planConstShares", planCostShares()},
        //            { "linkedPlanServices", linkedPlanServices()},
        //            {"_org", new JSchema{ Type = JSchemaType.String}},
        //            {"objectId", new JSchema{ Type = JSchemaType.String}},
        //            {"objectType", new JSchema{ Type = JSchemaType.String}},
        //            {"planType", new JSchema{ Type = JSchemaType.String} },
        //            {"creationDate", new JSchema{ Type = JSchemaType.String, Format="date-time"} }
        //        }
        //    };

        //    return res;
        //}

        //private JSchema planCostShares()
        //{
        //    JSchema pCS = new JSchema {
        //        Type = JSchemaType.Object,
        //        Properties = {
        //            { "deductible", new JSchema{ Type = JSchemaType.Integer} },
        //            { "_org", new JSchema{ Type = JSchemaType.String} },
        //            { "copay", new JSchema{ Type = JSchemaType.Integer} },
        //            { "objectId", new JSchema{ Type = JSchemaType.String} },
        //            { "objectType", new JSchema{ Type = JSchemaType.String} }
        //        }
        //    };
        //    return pCS;
        //}

        //private JSchema linkedPlanServices()
        //{
        //    return null;
        //}
    }
}
