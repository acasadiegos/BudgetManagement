using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Account, AccountCreationViewModel>();
            CreateMap<UpdateTransactionViewModel, Transaction>().ReverseMap();
        }
    }
}
