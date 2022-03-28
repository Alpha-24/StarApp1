using StarApp1.Models;

namespace StarApp1.Services
{
    public interface ILogin
    {
        LoginViewModel CheckLogin(LoginViewModel paramLogin);
        

    }
}
