using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models
{
    public interface IParameters
    {
        ParametersCollection ToParams();
    }
}
