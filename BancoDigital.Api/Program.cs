using BancoDigital.Api.Application.UseCases;
using BancoDigital.Api.Domain.Entidades;
using BancoDigital.Api.DTOs;
using BancoDigital.Api.Infrastructure;
using BancoDigital.Api.Infrastructure.Interceptors;
using BancoDigital.Api.Infrastructure.Outbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // EF Core + interceptor
            builder.Services.AddDbContext<BancoDigitalDbContext>((options =>
                             options.UseSqlite(builder.Configuration.GetConnectionString("BancoDigital"))
                             .AddInterceptors(builder.Services.BuildServiceProvider().GetRequiredService<OutboxSaveChangesInterceptor>())
                             ));

            // Interceptors
            builder.Services.AddSingleton<OutboxSaveChangesInterceptor>();

            // Services
            builder.Services.AddScoped<CriarContaUseCase>();
            builder.Services.AddScoped<TransferenciaUseCase>();

            // Dispatchers
            builder.Services.AddHostedService<OutboxDispatcher>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //Endpoints

            app.MapPost("/conta", async ([FromBody] CriarContaRequest request, CriarContaUseCase uc) =>
            {
                // Cria a conta no domínio
                var conta = new Conta(request.Id, request.SaldoInicial);
                await uc.Criar(conta);
                return Results.Created($"/conta/{conta.Id}", conta);
            }).WithOpenApi();

            app.MapPost("/transferencia", async (Guid origem, Guid destino, decimal valor, TransferenciaUseCase uc) =>
            {
                await uc.EfetuarTransferencia(origem, destino, valor);
                return Results.Ok();
            })
            .WithOpenApi();

            app.Run();
        }
    }
}
