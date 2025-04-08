using Microsoft.EntityFrameworkCore;
using Project_01.Data;
using Project_01.Domain.Clients;
using Project_01.Domain.Contracts;
using Project_01.Domain.Discounts;
using Project_01.Domain.Payments;
using Project_01.Domain.Products;
using Project_01.Interfaces;
using Project_01.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IContractService, ContractService>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();
        context.Database.Migrate();
        SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();

void SeedData(DatabaseContext context)
{
    if (!context.Clients.Any())
    {
        context.Clients.AddRange(
            new IndividualClient { Name = "Jan", LastName = "Kowalski", PESEL = "80010112345", Address = "Warszawa, ul. Główna 1", Email = "jan@example.com", PhoneNumber = "123456789", ClientType = ClientType.Individual },
            new IndividualClient { Name = "Anna", LastName = "Nowak", PESEL = "90020223456", Address = "Kraków, ul. Długa 2", Email = "anna@example.com", PhoneNumber = "234567890", ClientType = ClientType.Individual },
            new CorporateClient { Name = "ABC Sp. z o.o.", KRS = "0000123456", Address = "Poznań, ul. Biznesowa 3", Email = "kontakt@abc.com", PhoneNumber = "345678901", ClientType = ClientType.Corporate }
        );
    }

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product { Name = "Finanse Pro", Description = "Oprogramowanie do zarządzania finansami", Version = "2.0", Category = "Finanse", YearlyLicenseCost = 5000m },
            new Product { Name = "HR Manager", Description = "System zarządzania zasobami ludzkimi", Version = "1.5", Category = "HR", YearlyLicenseCost = 3000m }
        );
    }

    if (!context.Discounts.Any())
    {
        context.Discounts.AddRange(
            new Discount { Name = "Zniżka letnia", PercentageValue = 10, StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 8, 31) },
            new Discount { Name = "Zniżka dla nowych klientów", PercentageValue = 15, StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 12, 31) }
        );
    }

    context.SaveChanges();

    if (!context.Contracts.Any())
    {
        var client = context.Clients.First();
        var product = context.Products.First();

        var contract = new Contract
        {
            ClientId = client.Id,
            ProductId = product.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1),
            TotalPrice = product.YearlyLicenseCost,
            IsSigned = true,
            SupportYears = 1,
            Status = ContractStatus.Active
        };

        context.Contracts.Add(contract);
        context.SaveChanges();

        context.Payments.Add(new Payment
        {
            ContractId = contract.Id,
            Amount = contract.TotalPrice,
            PaymentDate = DateTime.Now,
            Status = PaymentStatus.Completed
        });

        context.SaveChanges();
    }
}