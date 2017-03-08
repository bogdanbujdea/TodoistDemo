using System;
using System.Reactive.Linq;
using System.Threading;
using Windows.UI.Xaml;
using ReactiveUI;
using TodoistDemo.ViewModels;

namespace TodoistDemo.Views
{
    public sealed partial class ItemsView : IViewFor<ItemsViewModel>
    {
        public ItemsView()
        {
            InitializeComponent();
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as ItemsViewModel;
            CreateBindings();
        }

        private void CreateBindings()
        {
            var context = SynchronizationContext.Current;
            this.WhenAnyValue(view => view.AuthToken.Text)
                .Where(token => string.IsNullOrWhiteSpace(token) == false)
                .Throttle(TimeSpan.FromSeconds(3))
                .ObserveOn(context)
                .Subscribe(async token =>
                {
                    if (string.IsNullOrWhiteSpace(token) || token.Length < 10) //don't know if token length can be different from 40
                    {
                        return;
                    }
                    await ViewModel.Sync();
                });

            this.WhenAnyValue(x => x.ViewModel.IsBusy)
                .Throttle(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(busy =>
                {
                    LoadingGrid.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
                    ProgressIndicator.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
                });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(ItemsViewModel),
            typeof(ItemsView),
            new PropertyMetadata(default(ItemsViewModel)));

        public ItemsViewModel ViewModel
        {
            get { return (ItemsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ItemsViewModel)value; }
        }
    }
}