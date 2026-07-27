using System.Text.Json;

namespace ejercicio_02_poo;

public class TareaDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Prioridad Prioridad { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public bool Completada { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool TieneVencimiento { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}

public class GestorTareas
{
    private List<Tarea> tareas = new List<Tarea>();

    public void Agregar(Tarea tarea)
    {
        tareas.Add(tarea);
    }

    public void Completar(int id)
    {
        var tarea = tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            tarea.Completada = true;
            Console.WriteLine($"✓ Tarea con ID {id} marcada como completada.");
        }
        else
        {
            Console.WriteLine($"✗ No se encontró la tarea con ID {id}.");
        }
    }

    public List<Tarea> ObtenerTodas() => tareas;

    public List<Tarea> ListarPorCategoria(string categoria)
    {
        return tareas.Where(t => t.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Tarea> ListarPorPrioridad(Prioridad prioridad)
    {
        return tareas.Where(t => t.Prioridad == prioridad).ToList();
    }

    public List<Tarea> ObtenerVencidas()
    {
        return tareas
            .OfType<TareaConVencimiento>()
            .Where(t => DateTime.Compare(t.FechaVencimiento.Date, DateTime.Now.Date) < 0 && !t.Completada)
            .Cast<Tarea>()
            .ToList();
    }

    public void Eliminar(int id)
    {
        var tarea = tareas.FirstOrDefault(t => t.Id == id);
        if (tarea != null)
        {
            tareas.Remove(tarea);
            Console.WriteLine($"✓ Tarea con ID {id} eliminada.");
        }
        else
        {
            Console.WriteLine($"✗ Tarea no encontrada.");
        }
    }

    public void GuardarEnJSON(string archivo)
    {
        try
        {
            var dtoList = tareas.Select(t => new TareaDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Prioridad = t.Prioridad,
                Categoria = t.Categoria,
                Completada = t.Completada,
                FechaCreacion = t.FechaCreacion,
                TieneVencimiento = t is TareaConVencimiento,
                FechaVencimiento = (t as TareaConVencimiento)?.FechaVencimiento
            }).ToList();

            string json = JsonSerializer.Serialize(dtoList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(archivo, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar JSON: {ex.Message}");
        }
    }

    public void CargarDeJSON(string archivo)
    {
        if (!File.Exists(archivo)) return;

        try
        {
            string json = File.ReadAllText(archivo);
            var dtoList = JsonSerializer.Deserialize<List<TareaDTO>>(json);

            if (dtoList == null) return;

            tareas.Clear();
            int maxId = 0;

            foreach (var dto in dtoList)
            {
                Tarea t;
                if (dto.TieneVencimiento && dto.FechaVencimiento.HasValue)
                {
                    t = new TareaConVencimiento
                    {
                        Id = dto.Id,
                        Titulo = dto.Titulo,
                        Descripcion = dto.Descripcion,
                        Prioridad = dto.Prioridad,
                        Categoria = dto.Categoria,
                        Completada = dto.Completada,
                        FechaCreacion = dto.FechaCreacion,
                        FechaVencimiento = dto.FechaVencimiento.Value
                    };
                }
                else
                {
                    t = new Tarea
                    {
                        Id = dto.Id,
                        Titulo = dto.Titulo,
                        Descripcion = dto.Descripcion,
                        Prioridad = dto.Prioridad,
                        Categoria = dto.Categoria,
                        Completada = dto.Completada,
                        FechaCreacion = dto.FechaCreacion
                    };
                }

                tareas.Add(t);
                if (dto.Id > maxId) maxId = dto.Id;
            }

            Tarea.ActualizarContador(maxId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Advertencia: No se pudo cargar {archivo} ({ex.Message})");
        }
    }
}