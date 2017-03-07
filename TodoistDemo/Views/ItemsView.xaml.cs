using System;
using System.Reactive.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Caliburn.Micro;
using ReactiveUI;
using TodoistDemo.ViewModels;

namespace TodoistDemo.Views
{
    public sealed partial class ItemsView: IViewFor<ItemsViewModel>
    {
        public ItemsView()
        {
            InitializeComponent();
            Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as ItemsViewModel;
            this.WhenAnyValue(view => view.AuthToken.Text)
                .Where(token => string.IsNullOrWhiteSpace(token) == false)
                .Throttle(TimeSpan.FromSeconds(3))
                .DoWhile(() => string.IsNullOrWhiteSpace(ViewModel.Username))
                .Subscribe(async token =>
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await ViewModel.Sync();
                    });
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