using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IFamilyMemberRepository
    {
        Task Add(FamilyMember familyMember);
        Task<FamilyMember?> GetById(Guid id);
    }

}
