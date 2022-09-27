using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorContracts.Dtos
{
    public class AuthorGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AuthorGuid { get; set; }
    }
}
