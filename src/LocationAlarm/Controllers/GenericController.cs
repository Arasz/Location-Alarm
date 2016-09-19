namespace LocationAlarm.Controllers
{
    public abstract class GenericController<TView, TViewModel, TModel> : BaseController
    {
        public TModel Model { get; set; }

        public TView View { get; set; }

        public TViewModel ViewModel { get; set; }

        protected GenericController(TView view, TViewModel viewModel, TModel model)
        {
            View = view;
            ViewModel = viewModel;
            Model = model;
        }
    }
}