namespace WebApi.Helpers;

using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Categories;
using WebApi.Models.Locations;
using WebApi.Models.Products;
using WebApi.Models.Roles;
using WebApi.Models.StockTransactions;
using WebApi.Models.Unities;
using WebApi.Models.Users;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, AuthenticateResponse>();
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateRequest, User>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<RoleCreateRequest, Role>();
        CreateMap<RoleUpdateRequest, Role>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<CategoryCreateRequest, Category>();
        CreateMap<CategoryUpdateRequest, Category>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<UnityCreateRequest, Unity>();
        CreateMap<UnityUpdateRequest, Unity>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<ProductCreateRequest, Product>();
        CreateMap<ProductUpdateRequest, Product>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<LocationCreateRequest, Location>();
        CreateMap<LocationUpdateRequest, Location>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));

        CreateMap<StockTransactionCreateRequest, StockTransaction>();
        CreateMap<StockTransactionUpdateRequest, StockTransaction>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    if (prop == null) { return false; }
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) { return false; }

                    return true;
                }
            ));
    }
}
