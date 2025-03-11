using Bogus;
using Microsoft.Extensions.Caching.Memory;
using mostlylucig.pagingtaghelper.sample.Models;

namespace mostlylucig.pagingtaghelper.sample.Services;



public class DataFakerService(IMemoryCache memoryCache)
{
    public async  Task<List<FakeDataModel>?> GenerateData(int count)
    {
       return await memoryCache.GetOrCreateAsync($"fakeData_{count}", async entry =>
       {
           entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
           return await Generate(count);
    
       });
    }
    
    private async Task<List<FakeDataModel>?> Generate(int count)
    {
        var faker = new Faker<FakeDataModel>()
            .RuleFor(f => f.Id, f => f.IndexFaker + 1)
            .RuleFor(f => f.Name, f => f.Name.FullName())
            .RuleFor(f => f.Description, f => f.Lorem.Sentence())
            .RuleFor(f => f.CompanyName, f => f.Company.CompanyName())
            .RuleFor(f => f.CompanyAddress, f => f.Address.StreetAddress())
            .RuleFor(f => f.CompanyCity, f => f.Address.City())
            .RuleFor(f => f.CompanyCountry, f => f.Address.Country())
            .RuleFor(f => f.CompanyEmail, f => f.Internet.Email())
            .RuleFor(f => f.CompanyPhone, f => f.Phone.PhoneNumber());

        return await Task.Run(() => faker.Generate(count));
    }
}