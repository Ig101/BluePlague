using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;

namespace ProjectArena.Domain.Identity.Entities
{
    public class Role : MongoRole
    {
        public Role(string name)
            : base(name)
        {
        }
    }
}