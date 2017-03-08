using Caliburn.Micro;

namespace TodoistDemo.ViewModels
{
    public class ViewModelBase : Screen
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }
    }
}