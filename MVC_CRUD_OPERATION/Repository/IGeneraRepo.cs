using MVC_CRUD_OPERATION.Models;
using System.Collections.Generic;

namespace MVC_CRUD_OPERATION.Repository
{
    public interface IGeneraRepo
    {
       List<Genre> GetAllOrderdByName();
        Genre GetBy(int id);
  

    }
}
