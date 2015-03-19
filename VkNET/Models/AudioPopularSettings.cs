using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNET.Extensions;

namespace VkNET.Models
{
    public class AudioPopularSettings : IParameters
    {

        public bool OnlyEng { get; set; }
        public AudioGenre Genre { get; set; }

        public ParametersCollection ToParams()
        {
            return new ParametersCollection
            {
                {"only_eng" , OnlyEng.ToInt()},
                {"genre_id" , (int)Genre},
            };
        }
    }
}
