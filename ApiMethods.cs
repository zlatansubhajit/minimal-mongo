
namespace minimal_api
{
    public class ApiMethods
    {
        public static async Task<IResult> GetCompanies(IMongoCollection<Company> collection)
        {
            return TypedResults.Ok(await collection.Find(Builders<Company>.Filter.Empty).ToListAsync());
        }

        public static async Task<IResult> AddCompanies(IMongoCollection<Company> collection, Company company)
        {
            company = company with { Id = ObjectId.Empty };
            await collection.InsertOneAsync(company);
            return TypedResults.Ok(company);
        }

        public static async Task<IResult> UpdateCompany(IMongoCollection<Company> collection, Company company)
        {
            await collection.ReplaceOneAsync(x => x.Id == company.Id, company);
            return TypedResults.Ok(company);
        }

        public static async Task<IResult> DeleteCompany(IMongoCollection<Company> collection, ObjectId companyId)
        {
            await collection.DeleteOneAsync(x => x.Id == companyId);
            return TypedResults.Ok(companyId);
        }

        public static async Task<IResult> GetOffices(IMongoCollection<Company> collection, ObjectId companyId)
        {
            var offices = await collection.Find(
                Builders<Company>.Filter.Eq(x => x.Id, companyId))
                .Project(x => x.Offices)
                .FirstOrDefaultAsync();
            return TypedResults.Ok(offices);

        }
    }

    public class MemberMethods
    {
        public static async Task<IResult> GetMembers(IMongoCollection<Member> collection)
        {
            return TypedResults.Ok(await collection.Find(Builders<Member>.Filter.Empty).ToListAsync());
        }

        public static async Task<IResult> GetMembersByFilter(IMongoCollection<Member> collection, ObjectId? id, string? name, string? email, long? phone, string? gymName)
        {
            if(String.IsNullOrWhiteSpace(id.ToString())  && String.IsNullOrWhiteSpace(name) && String.IsNullOrWhiteSpace(email) && phone.GetValueOrDefault(0)==0 && String.IsNullOrWhiteSpace(gymName))
            {
                return TypedResults.BadRequest("No filter condition provided");
            }
            var builder = (Builders<Member>.Filter);
            var filter = builder.Empty;
            var idFilter = builder.Eq(x => x.Id, id);
            var nameFilter = builder.Eq(x => x.Name, name);
            var emailFilter = builder.Eq(x => x.Email, email);
            var phoneFilter = builder.Eq(x => x.Phone, phone);
            var gymFilter = builder.Eq(x => x.GymName, gymName);
            if (!String.IsNullOrWhiteSpace(name))
            {
                filter = nameFilter;
            }
            if (!String.IsNullOrWhiteSpace(email))
            {
                filter &= emailFilter; 
            }
            if( phone != null )
            {
                filter &= phoneFilter;
            }
            if(!String.IsNullOrWhiteSpace(gymName))
            {
                filter &= gymFilter;
            }
            return TypedResults.Ok(await collection.Find(builder.And(filter)).ToListAsync());
        }

        public static async Task<IResult> AddMember(IMongoCollection<Member> collection, Member member)
        {
            member = member with { Id = ObjectId.Empty };
            await collection.InsertOneAsync(member);
            return TypedResults.Ok(member);
        }
    }
}
