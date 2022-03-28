using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarApp1.Models;
using StarApp1.Controllers;

namespace StarApp1.Models
{
    public class StarUserDbContext : DbContext
    {
        

        public StarUserDbContext(DbContextOptions<StarUserDbContext> options) : base(options)
        {
            
        }
        public  void ExecuteResult(ActionContext context)
        {
            var Response = context.HttpContext.Response;
           
        }
       
        public DbSet<StarApp1.Models.LoginViewModel> LoginViewModel { get; set; }
        public DbSet<StarApp1.Models.RegisterViewModel> RegisterViewModel { get; set; }
        public DbSet<StarApp1.Models.UserAdmin> UserAdmin { get; set; }
        public DbSet<StarApp1.Controllers.UpdateViewModel> UpdateViewModel { get; set; }
        public DbSet<StarApp1.Models.PasswordReset> PasswordReset { get; set; }
        
    }

}