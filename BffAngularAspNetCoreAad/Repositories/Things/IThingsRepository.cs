using System.Collections.Generic;
using BffAngularAspNetCoreAad.Models;

namespace BffAngularAspNetCoreAad.Repositories.Things
{
    public interface IThingsRepository
    {
        Thing GetSingle(int id);
        Thing Add(Thing item);
        void Delete(int id);
        Thing Update(int id, Thing item);
        ICollection<Thing> GetAll();
        int Count();
    }
}
