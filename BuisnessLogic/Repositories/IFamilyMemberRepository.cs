using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IFamilyMemberRepository
    {
        Task Add(FamilyMember familyMember);
        Task<FamilyMember?> GetById(Guid userId, Guid id);

        Task<List<FamilyMember>> GetAll(Guid userId);

        Task<List<FamilyMember>> GetAllScalar(Guid userID);
    }

}
