using SimpleCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCRUD.Data
{
    public interface ICrudRepository
    {
        Task<User> Register(User user);
        Task<bool> Delete(int id);
        List<User> GetUsers();
    }
}
