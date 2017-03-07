using ReactiveUI;

namespace TodoistDemo.Core.Communication.ApiModels
{
    public class BindableItem: ReactiveObject
    {
        private bool _checked;
        public int Id { get; set; }

        public string Content { get; set; }

        public bool Checked
        {
            get { return _checked; }
            set { this.RaiseAndSetIfChanged(ref _checked, value); }
        }
    }
}