using DataAccess.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IFamilyMemberRepository
    {
        Task Add(FamilyMember familyMember);
        Task<FamilyMember> GetById(Guid userId, Guid id);
    }

}
