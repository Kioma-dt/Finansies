namespace UI.Views.Service
{
    public class ViewService
        : IViewService
    {
        private readonly IServiceProvider _service;

        public ViewService(IServiceProvider service)
        {
            _service = service;
        }

        public TView GetView<TView>() 
            where TView : View
        {
            return  _service.GetRequiredService<TView>();
        }
    }
}
