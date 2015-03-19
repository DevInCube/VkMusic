using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Extensions;

namespace VkNET.Models
{

    public enum AudioSortOrder
    {
        ByDateAdded = 0,
        ByDuration = 1,
        ByPopularity = 2,
    }

    public class AudioSearchSettings : IParameters
    {

        public bool AutoComplete { get; set; }
        public bool Lyrics { get; set; }
        public bool PerformerOnly { get; set; }
        public AudioSortOrder Sort { get; set; }
        public bool SearchOwn { get; set; }

        public ParametersCollection ToParams()
        {
            return new ParametersCollection()
            {
                {"auto_complete", AutoComplete.ToInt()},
                {"lyrics", Lyrics.ToInt()},
                {"performer_only", PerformerOnly.ToInt()},
                {"sort", (int)Sort},
                {"search_own", SearchOwn.ToInt()},
            };
        }
    }
}
