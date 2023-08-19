using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Services.Interfaces;
using Pexita.Services;
using Pexita.Additionals;
using Pexita.Utility;
using Pexita.Utility.Validators;
using FluentValidation;
using Pexita.Data.Entities.Products;
using Pexita.Data.Entities.Brands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddDbContext<AppDBContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbTwo")));

builder.Services.AddTransient<IValidator<ProductCreateVM>, ProductCreateValidation>();
builder.Services.AddTransient<IValidator<ProductUpdateVM>, ProductUpdateValidation>();
builder.Services.AddTransient<IValidator<BrandCreateVM>, BrandCreateValidation>();
builder.Services.AddTransient<IValidator<BrandUpdateVM>, BrandUpdateValidation>();


builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IBrandService, BrandService>();
builder.Services.AddTransient<ITagsService, TagsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPexitaTools, PexitaTools>();
builder.Services.AddTransient<IIranAPI, IranAPI>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
InitialData.Seed(app);
app.Run();
