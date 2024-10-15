using HotChocolate;
using HotChocolate.Types;
using MongoDB.Bson;
using SOP.Entity;
using SOP.Services;
using SOP.GraphQL;


public class WagonType : ObjectType<WagonResource>
{
    protected override void Configure(IObjectTypeDescriptor<WagonResource> descriptor)
    {
        descriptor.Field(w => w.Id)
            .Type<IdType>(); // Используйте IdType для ObjectId

        descriptor.Field(w => w.Links)
            .Ignore(); // Игнорируйте ссылки, если они не нужны в ответе
    }
}

public class WagonSchema : ObjectType<WagonResource>
{
    protected override void Configure(IObjectTypeDescriptor<WagonResource> descriptor)
    {
        descriptor.Field(t => t.Cargo).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Capacity).Type<NonNullType<IntType>>();
        descriptor.Field(t => t.Loaded).Type<NonNullType<IntType>>();
        descriptor.Field(t => t.IsLoaded).Type<NonNullType<BooleanType>>();

        // Определение метода для запроса по ID
        descriptor.Field("wagon")
            .Argument("id", a => a.Type<NonNullType<IdType>>())
            .ResolveWith<Query>(q => q.Wagon(default!)) // Убедитесь, что здесь используется правильное разрешение
            .Description("Получить вагон по ID.");
    }
}
