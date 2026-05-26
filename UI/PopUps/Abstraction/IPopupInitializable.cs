namespace UI.PopUps.Abstraction
{
    public interface IPopUpInitializable<in TArgs>
    {
        Task Initialize(TArgs args);
    }
}
