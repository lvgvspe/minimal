using CatalogoApi.Context;
using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.ApiEndpoints;

public static class CategoriasEndpoints
{
    public static void MapCategoriasEndpoints(this WebApplication app)
    {
        app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
        {
            db.Categorias.Add(categoria);
            await db.SaveChangesAsync();

            return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
        }).WithTags("Categorias");

        app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync())
            .WithTags("Categorias")
            .RequireAuthorization();

        app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Categorias.FindAsync(id)
            is Categoria categoria ? Results.Ok(categoria) : Results.NotFound();
        }).WithTags("Categorias");

        app.MapPut("/categorias/{id:int}", async (int id, AppDbContext db, Categoria categoria) =>
        {
            if (categoria.CategoriaId != id) return Results.BadRequest();

            var categoriaDb = await db.Categorias.FindAsync(id);

            if (categoriaDb is null) return Results.NotFound();

            categoriaDb.Nome = categoria.Nome;
            categoriaDb.Descricao = categoria.Descricao;

            await db.SaveChangesAsync();
            return Results.Ok(categoriaDb);
        }).WithTags("Categorias");

        app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
        {
            var categoriaDb = await db.Categorias.FindAsync(id);
            if (categoriaDb is null) return Results.NotFound();
            db.Categorias.Remove(categoriaDb);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithTags("Categorias");

        app.MapGet("/categoriaprodutos", async (AppDbContext db) =>
        await db.Categorias.Include(c => c.Produtos.OrderBy(p => p.ProdutoId)).ToListAsync())
            .Produces<List<Categoria>>(StatusCodes.Status200OK)
            .WithTags("Categorias");
    }
}
