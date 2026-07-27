namespace ejercicio_02_poo;

public class Tarea : IExportable
{
    private static int _contadorId = 1;

    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public Prioridad Prioridad { get; set; }
    public string Categoria { get; set; }
    public bool Completada { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Constructor para tareas nuevas
    public Tarea(string titulo, string descripcion, Prioridad prioridad, string categoria)
    {
        Id = _contadorId++;
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        Categoria = categoria;
        Completada = false;
        FechaCreacion = DateTime.Now;
    }

    // Constructor vacío para deserialización JSON
    public Tarea() { }

    public static void ActualizarContador(int ultimoId)
    {
        if (ultimoId >= _contadorId)
        {
            _contadorId = ultimoId + 1;
        }
    }

    public virtual void MostrarInfo()
    {
        string estado = Completada ? "[X] Completada" : "[ ] Pendiente";
        Console.WriteLine($"ID: {Id} | {estado} | Título: {Titulo} | Prioridad: {Prioridad} | Cat: {Categoria}");
        Console.WriteLine($"   Descripción: {Descripcion}");
        Console.WriteLine($"   Creada: {FechaCreacion:dd/MM/yyyy HH:mm}");
    }

    public string Exportar()
    {
        return $"{Id}|{Titulo}|{Prioridad}|{Completada}";
    }
}