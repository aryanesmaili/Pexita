using FluentValidation;
using Pexita.Data.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitTest.FakeServices
{
    internal class FakeProductvalidation : AbstractValidator<ProductCreateVM>
    {
        public FakeProductvalidation()
        {
                
        }
    }
}
