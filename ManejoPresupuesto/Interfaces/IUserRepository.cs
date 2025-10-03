using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
        Task<User> GetUserByEmail(string normalizedEmail);
    }
}
