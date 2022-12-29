using AutoMapper;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Core.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookfiy_WepApp.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //category
            CreateMap<Category , CategoryViewModel>();
            CreateMap<CategoryFormViewModel , Category>().ReverseMap();
            CreateMap<Category, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
                


            //Author
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            //Books
            CreateMap<BookFormViewModel, Book>()
                .ReverseMap()
                .ForMember(dest => dest.Categories , opt =>opt.Ignore());
            CreateMap<Book , BookViewModel>()
                .ForMember(dest => dest.Author , opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.Categories , opt => opt.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));

            CreateMap<BookCopy,BookCopyViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src=>src.Book!.Title));
            CreateMap<BookCopy, BoooCopyFormViewModel>();

            //user

            CreateMap<ApplicationUser, UsersViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>()
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
                .ReverseMap();

            //subscroper
            CreateMap<Subscriper, SubscriperFormViewModel>().ReverseMap();
            CreateMap<Subscriper, SubscriberSearchViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ReverseMap();
            CreateMap<Subscriper, SubscriperViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate!.Name))
                .ReverseMap(); 

            //Governorea & Area
            CreateMap<Governorate, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            CreateMap<Area, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
        }
    }
}
