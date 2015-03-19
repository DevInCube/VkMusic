using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models
{
    public class ParametersCollection : Dictionary<string, object>, IParameters
    {

        public ParametersCollection()
        {
            //
        }

        public ParametersCollection(ParametersCollection pc) 
            : base(pc as Dictionary<string, object>)
        {
            //
        }

        public ParametersCollection MergeWith(IParameters coll)
        {
            ParametersCollection newCol = new ParametersCollection(this);
            if (coll != null)
                coll.ToParams().ToList().ForEach(x => newCol[x.Key] = x.Value);
            return newCol;
        }

        public ParametersCollection ToParams()
        {
            return this;
        }

        public override string ToString()
        {
            NameValueCollection parameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (string key in this.Keys)
                parameters[key] = this[key] == null ? "" : this[key].ToString();
            return parameters.ToString();
        }
    }
}
