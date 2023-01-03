
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

#region  Criar novo projeto e add swagger 
// crie o projeto para minimal api dotnet new web
// Add Swagger 
// dotnet add package Swashbuckle.AspNetCore 
// em launchSettings.Json em profiles add // "launchUrl": "swagger" 

/* em   programa.cs substitua todo o codigo por este 

using Microsoft.AspNetCore.Builder;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.MapGet("/", () => "Hello World!");

app.UseSwaggerUI();
app.Run();

*/

// compile o programa dotnet build
// rode o programa dotnet run
// rode o programa dotnet watch run
// acesse o programa http://localhost:5265/swagger/index.html obs; sua porta pode ser diferente da minha, confira.

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Contexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoBD"));
});

builder.Services.AddAutoMapper(typeof(CMapper));

var app = builder.Build();

app.UseSwagger();
//app.MapGet("/", () => "Hello World!");


// Get all Cursos 
app.MapGet("/cursos", async (Contexto db) =>
{

    try
    {
        
   
    //return db.Cursos.ToList();
    var result = await db.Cursos.ToListAsync();
    return Results.Ok(result);
    //return Results.BadRequest();
    //return Results.NoContent();
    //return Results.NotFound();
     }
    catch (System.Exception ex)
    {
        
        return Results.Problem(ex.Message);
    }
});

// Post curso 

// app.MapPost("/cursos", async (Curso curso, Contexto db) =>
// {
//           db.Cursos.Add(curso);
//          await db.SaveChangesAsync();

//         return Results.Created($"/cursos/{curso.CursoId}", curso);
// });

//app.MapPost("/cursos", async ([FromBody] CursoDto cursoDto, [FromServices] Contexto db, [FromServices] IMapper mapper) =>
app.MapPost("/cursos", async ( CursoDto cursoDto, Contexto db, IMapper mapper) =>
{
    try
    {
    var novoCurso = mapper.Map<Curso>(cursoDto);

    db.Cursos.Add(novoCurso);
    await db.SaveChangesAsync();

    var result = mapper.Map<CursoDto>(novoCurso);
    return Results.Created($"/cursos/{result.CursoId}", result);

     }
    catch (System.Exception ex)
    {
        //throw new InvalidOperationException();
        //return Results.StatusCode(500);
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/cursos/{cursoId}", async (int cursoId, Contexto db, IMapper mapper) =>
 {
    var curso = await db.Cursos.FindAsync(cursoId);
    if(curso == null)
     {
        return Results.NotFound();
     }

     var resultado = mapper.Map<CursoDto>(curso);
     return Results.Ok(resultado);
});

// Put Atualizar 
app.MapPut("/cursos/{cursoId}", async (int cursoId, CursoDto cursoDto, Contexto db, IMapper mapper) =>
 {
    var curso = await db.Cursos.FindAsync(cursoId);
    if(curso == null)
     {
        return Results.NotFound();
     }

     curso.NomeCurso = cursoDto.NomeCurso;
     curso.Duracao = cursoDto.Duracao;
     curso.TipoCurso = (int)cursoDto.TipoCurso;
     await db.SaveChangesAsync();

     var resultado = mapper.Map<CursoDto>(curso);
     return Results.Ok(resultado);
});

// Delete

app.MapDelete("/cursos/{cursoId}", async (int cursoId, Contexto db, IMapper mapper) =>
 {
    var curso = await db.Cursos.FindAsync(cursoId);
    if(curso == null)
     {
        return Results.NotFound();
     }
     db.Cursos.Remove(curso);
     await db.SaveChangesAsync();

     var resultado = mapper.Map<CursoDto>(curso);
     return Results.Ok(resultado);
});


app.UseSwaggerUI();
app.Run();

public class CMapper : Profile // import Automapper
{
    public CMapper()
    {
        CreateMap<Curso, CursoDto>();
        CreateMap<CursoDto, Curso>();
    }
}


#region CLASSES "Models"

public class Curso
{
    [Key] // import data annotation
    public int CursoId { get; set; }
    public string NomeCurso { get; set; } = string.Empty;
    public int Duracao { get; set; }
      
    public int TipoCurso { get; set; }
}
public class CursoDto
{

    public int CursoId { get; set; }
    public string NomeCurso { get; set; } = string.Empty;
    public int Duracao { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TipoCurso TipoCurso { get; set; }
}

public enum TipoCurso
{
    Desenvolvimento = 1,
    Programacao = 2,
    Frontend = 3,
    Backend = 4,
    Mobile = 5,
    Fullstack = 6
}

#endregion


#region  Conexao com banco de dados "Contexto"
// ctr + p depois > procura nuget package manager e adicione entity framework core tools e o sqlserver 
// ou adicinione via terminal dotnet add package microsoft.entityframeworkcore.tools e microsoft.entityframeworkcore.sqlserver
// appSettings.json  Add sua  Server=localhost;Database=CampusBD;Trust Server Certificate=true;Trusted_Connection=True;


// programa.cs // linha 39
//builder.Services.AddDbContext<Contexto>( options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoBD")));


public class Contexto : DbContext // using Microsoft.EntityFrameworkCore;
{
    public Contexto(DbContextOptions<Contexto> options) : base(options)
    {

    }
    // public DbSet<Cursos> Cursos{get; set;}
    public DbSet<Curso> Cursos => Set<Curso>();
}


//Add Injecao de dependencia



#endregion

