using MyStorage.Models;
using MyStorage.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyStorage.ViewModels
{
    public class ProductViewModel : BaseProductViewModel
    {
        public Command LoadProductCommand { get; }
        public ObservableCollection<ProductInfo> ProductInfos { get; }

        public Command AddProductCommand { get; }
        public Command ProductTappedEdit { get; }
        public Command ProductTappedDelete { get; }
        public Command ClearProductCommand { get; }
        public ProductViewModel(INavigation _navigation)
        {
            LoadProductCommand = new Command(async () => await ExecuteLoadProductCommand());
            ProductInfos = new ObservableCollection<ProductInfo>();
            AddProductCommand = new Command(OnAddProduct);
            ProductTappedEdit = new Command<ProductInfo>(OnEditProduct);
            ProductTappedDelete = new Command<ProductInfo>(OnDeleteProduct);
            ClearProductCommand = new Command(ClearProduct);
            Navigation = _navigation;
        }
        public void OnApearing()
        {
            IsBusy = true;
        }
        async Task ExecuteLoadProductCommand()
        {
            IsBusy = true;

            try
            {
                ProductInfos.Clear();
                var prodList = await App.ProductService.GetProductAsync();
                foreach (var prod in prodList)
                {
                    ProductInfos.Add(prod);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
            }
            
        }
        private async void OnAddProduct(object obj)
        {
            await Shell.Current.GoToAsync(nameof(AddProductPage));
        }

        private async void OnEditProduct(ProductInfo prod)
        {
            await Navigation.PushAsync(new AddProductPage(prod));
        }
        private async void OnDeleteProduct(ProductInfo prod)
        {
            if(prod == null)
            {
                return;
            }

            await App.ProductService.DeleteProductAsync(prod.ProductId);
            await ExecuteLoadProductCommand();
        }
        private async void ClearProduct()
        {
            ProductInfos.Clear();
        }
    }
}
