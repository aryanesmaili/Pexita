using Pexita.Data.Entities.Comments;
using Pexita.Data.Entities.Products;
using Pexita.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakeProductService : IProductService
    {
        public bool AddCommentToProduct(int ProductID, CommentsModel comment)
        {
            throw new NotImplementedException();
        }

        public bool AddProduct(ProductCreateVM product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }

        public ProductInfoVM GetProductByID(int id)
        {
            throw new NotImplementedException();
        }

        public List<ProductInfoVM> GetProducts()
        {
            throw new NotImplementedException();
        }

        public List<ProductInfoVM> GetProducts(int count)
        {
            throw new NotImplementedException();
        }

        public ProductInfoVM ProductModelToInfoVM(ProductModel model)
        {
            throw new NotImplementedException();
        }

        public ProductInfoVM UpdateProductInfo(int id, ProductUpdateVM product)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProductRate(int ProductID, int rate)
        {
            throw new NotImplementedException();
        }
    }
}
