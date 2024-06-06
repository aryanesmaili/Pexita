using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Services.Interfaces;
using Pexita.Services;
using Pexita.Utility;
using FluentValidation.AspNetCore;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddDbContext<AppDBContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbTwo")));

// automatically adds all validators of this project to DI pool.
var assembly = typeof(Program).Assembly;
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IBrandService, BrandService>();
builder.Services.AddTransient<ITagsService, TagsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPexitaTools, PexitaTools>();
builder.Services.AddTransient<IIranAPI, IranAPI>();
builder.Services.AddTransient<IPaymentService, PaymentService>();

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
