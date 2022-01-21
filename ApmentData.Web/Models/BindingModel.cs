using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApmentData.Web.Models
{
    public class BindingModel
    {
        public Task<List<Property>> Properties { get; set; }
        public Task<List<Management>> Managements { get; set; }
    }
}
