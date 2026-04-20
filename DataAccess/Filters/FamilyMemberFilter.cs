using DataAccess.Entities;

namespace DataAccess.Filters
{
    public class FamilyMemberFilter : IFilter
    {
        Guid _familyMemberId;

        public FamilyMemberFilter(Guid familyMemberId)
        {
            _familyMemberId = familyMemberId;
        }
        public bool Apply(Transaction transaction)
        {
            return transaction.FamilyMemberId == _familyMemberId;
        }
    }
}
