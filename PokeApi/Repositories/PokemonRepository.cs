using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PokeApi.Data;
using PokeApi.Model;

namespace PokeApi.Repositories
{
  public class PokemonRepository : BaseRepository<Pokemon>, IPokemonRepository
  {
    public PokemonRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task AddPokemons(List<Pokemon> pokemons)
    {
      foreach (var poke in pokemons)
      {
        if (!dbSet.Where(p => p.Number == poke.Number).Any())
        {
          dbSet.Add(poke);
        }
      }

      await context.SaveChangesAsync();
    }

    public Pokemon GetPokemonByNumber(int number)
    {
      return dbSet
        .Where(p => p.Number == number)
        .Include(n => n.NextEvolution)
          .ThenInclude(n => n.NextPokemon)
        .Include(p => p.PrevEvolution)
          .ThenInclude(n => n.PrevPokemon)
        .Include(w => w.Weaknesses)
          .ThenInclude(t => t.TypeElement)
        .Include(t => t.Type)
          .ThenInclude(t => t.TypeElement)
        .SingleOrDefault();
    }

    public async Task<Pokemon> GetPokemonByNumberAsync(int number)
    {
      return await dbSet
        .Where(p => p.Number == number)
        .Include(n => n.NextEvolution)
          .ThenInclude(n => n.NextPokemon)
        .Include(p => p.PrevEvolution)
          .ThenInclude(n => n.PrevPokemon)
        .Include(w => w.Weaknesses)
          .ThenInclude(t => t.TypeElement)
        .Include(t => t.Type)
          .ThenInclude(t => t.TypeElement)
        .SingleOrDefaultAsync();
    }

    public List<Pokemon> GetPokemons()
    {
      return dbSet
        .OrderBy(p => p.Number)
        .Include(t => t.Type)
          .ThenInclude(t => t.TypeElement)
        .Include(w => w.Weaknesses)
          .ThenInclude(t => t.TypeElement)
        .Include(n => n.NextEvolution)
        .Include(p => p.PrevEvolution)
        .ToList();
    }

    public async Task<List<Pokemon>> GetPokemonsAsync()
    {
      return await dbSet
        .OrderBy(p => p.Number)
        .Include(t => t.Type)
          .ThenInclude(t => t.TypeElement)
        .Include(w => w.Weaknesses)
          .ThenInclude(t => t.TypeElement)
        .Include(n => n.NextEvolution)
        .Include(p => p.PrevEvolution)
        .ToListAsync();
    }

    public async Task<IQueryable<Pokemon>> GetPokemonsByNameAsync(string name = "")
    {
      var data = Task.Run(() => 
        dbSet
          .Include(t => t.Type)
            .ThenInclude(t => t.TypeElement)
          .Include(w => w.Weaknesses)
            .ThenInclude(t => t.TypeElement)
          .Include(n => n.NextEvolution)
            .ThenInclude(n => n.NextPokemon)
          .Include(p => p.PrevEvolution)
            .ThenInclude(n => n.PrevPokemon)
          .Where(p =>
            string.IsNullOrEmpty(name) ||
            p.Name.ToUpper().Contains(name.ToUpper()) ||
            p.Number.ToString().Contains(name)
          )
          .OrderBy(p => p.Number)
          .AsQueryable()
      );

      return await data;
    }

    public List<PokemonSerialize> GetPokemonsSerialized()
    {
      var json = File.ReadAllText("Pokemons.json");
      var pokemons = JsonConvert.DeserializeObject<List<PokemonSerialize>>(json);

      return pokemons;
    }
  }
}