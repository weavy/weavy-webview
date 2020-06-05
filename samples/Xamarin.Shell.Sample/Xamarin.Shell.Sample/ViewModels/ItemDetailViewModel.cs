using System;

using Xamarin.Shell.Sample.Models;

namespace Xamarin.Shell.Sample.ViewModels {
    public class ItemDetailViewModel : BaseViewModel {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
