using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pexita.Data;
using Pexita.Data.Entities.Authentication;
using Pexita.Data.Entities.Events;
using Pexita.Data.Entities.Payment;
using Pexita.Data.Entities.SMTP;
using Pexita.Services;
using Pexita.Services.Interfaces;
using Pexita.Utility.MapperConfigs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(TagMapper));
builder.Services.AddAutoMapper(typeof(ProductMapper));
builder.Services.AddAutoMapper(typeof(BrandMapper));
builder.Services.AddAutoMapper(typeof(CartMapper));
builder.Services.AddAutoMapper(typeof(NewsLettersMapper));
builder.Services.AddAutoMapper(typeof(TokenMapper));
builder.Services.AddAutoMapper(typeof(UserMapper));

//builder.Services.AddTransient<AutoMapperConfig>();


builder.Services.AddDbContext<AppDBContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDB")));

// Add the SMTP service to be able to send emails
builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Add the Event driven system services
builder.Services.AddSingleton<EventDispatcher>();
builder.Services.AddScoped<ProductAvailableEventHandler>();
builder.Services.AddScoped<BrandNewProductEventHandler>();

// Automatically adds all validators of this project to DI pool.
var assembly = typeof(Program).Assembly;
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IBrandService, BrandService>();
builder.Services.AddTransient<ITagsService, TagsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPexitaTools, PexitaTools>();
builder.Services.AddTransient<IIranAPI, IranAPI>();
builder.Services.AddTransient<BrandPicURLResolver>();
builder.Services.AddTransient<INewsletterService, NewsletterService>();

builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection("PaymentSettings"));

builder.Services.AddTransient<IPaymentService>(provider =>
{
    var paymentSettings = provider.GetRequiredService<IOptions<PaymentSettings>>().Value;
    var context = provider.GetRequiredService<AppDBContext>();
    var mapper = provider.GetRequiredService<IMapper>();
    return new PaymentService(paymentSettings.APIKey, paymentSettings.CallbackAddress, paymentSettings.isTest, context, mapper);
});

// Authentication 
var jwtSettings = new JwtSettings();
builder.Configuration.Bind(nameof(JwtSettings), jwtSettings);
builder.Services.AddSingleton(jwtSettings);

var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey!);

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtbearer =>
{
    jwtbearer.RequireHttpsMetadata = true;
    jwtbearer.SaveToken = true;
    jwtbearer.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("Brand", policy => policy.RequireRole("admin", "Brand"));
    options.AddPolicy("AllUsers", policy => policy.RequireRole("admin", "user", "Brand"));
    options.AddPolicy("OnlyUsers", policy => policy.RequireRole("admin", "user"));
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    var productAvailableEventHandler = scopedServices.GetRequiredService<ProductAvailableEventHandler>();
    var BrandAvailableEventHandler = scopedServices.GetRequiredService<BrandNewProductEventHandler>();
    var event_dispatcher = scopedServices.GetRequiredService<EventDispatcher>();
    event_dispatcher.RegisterHandlerAsync<ProductAvailableEvent>(productAvailableEventHandler.Handle);
    event_dispatcher.RegisterHandler<BrandNewProductEvent>(BrandAvailableEventHandler.Handle);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
