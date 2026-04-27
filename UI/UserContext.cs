namespace UI
{
    public interface IUserContext
    {
        Guid UserId { get; }
        public void SetUserId(Guid id);
    }
    public class UserContext : IUserContext
    {
        Guid _userId = new Guid("00000000-0000-0000-0000-000000000001");
        public Guid UserId => _userId;

        public void SetUserId(Guid id)
        {
            _userId = id;
        }
    }
}
