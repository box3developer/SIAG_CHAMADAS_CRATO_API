using PATINHAS_RFID_API;
using PATINHAS_RFID_API.Repositories.Implementations;
using PATINHAS_RFID_API.Repositories.Interfaces;
using PATINHAS_RFID_API.Services.Implementations;
using PATINHAS_RFID_API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

Global.SiagAPI = builder.Configuration.GetConnectionString("siagAPI") ?? "";

// Add services to the container.
// REPOS
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<ISetorRepository, SetorRepository>();
builder.Services.AddScoped<IPalletRepository, PalletRepository>();
builder.Services.AddScoped<IOperadorRepository, OperadorRepository>();
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<ICheckListRepository, CheckListRepository>();
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped<IChamadaAtivaRepository, ChamadaAtivaRepository>();
builder.Services.AddScoped<IChamadaTarefaRepository, ChamadaTarefaRepository>();
builder.Services.AddScoped<IAreaArmazenagemRepository, AreaArmazenagemRepository>();
builder.Services.AddScoped<IAtividadeRotinaRepository, AtividadeRotinaRepository>();
builder.Services.AddScoped<IAtividadeRejeicaoRepository, AtividadeRejeicaoRepository>();
builder.Services.AddScoped<ICheckListOperadorRepository, CheckListOperadorRepository>();
builder.Services.AddScoped<IChamadaRepository, ChamadaRepository>();
builder.Services.AddScoped<IAtividadeTarefaRepository, AtividadeTarefaRepository>();

// SERVICES
builder.Services.AddScoped<IOperadorService, OperadorService>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();

builder.Services.AddCors(options =>
{

    options.AddPolicy("MyPolicy",
                        policy =>
                        {
                            policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("MyPolicy");

app.MapControllers();

app.Run();
