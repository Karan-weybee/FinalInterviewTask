using AutoMapper;
using PartyProductCore.Entities;
using PartyProductCore.Models;

namespace MoviesApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Parties, PartyDTO>().ReverseMap();
            CreateMap<Products, ProductDTO>().ReverseMap();
            CreateMap<AssignParties, AssignPartyDTO>().ReverseMap();
            CreateMap<ProductRates, ProductRateDTO>().ReverseMap();
            CreateMap<Invoices, InvoiceDTO>().ReverseMap();

        }
    }
}
