namespace ejercicio_02_poo;

public class TareaConVencimiento : Tarea
{
    public DateTime FechaVencimiento { get; set; }

    // Propiedad calculada en tiempo real
    public int DiasRestantes => (FechaVencimiento.Date - DateTime.Now.Date).Days;

    public TareaConVencimiento(string titulo, string descripcion, Prioridad prioridad, string categoria, DateTime fechaVencimiento)
        : base(titulo, descripcion, prioridad, categoria)
    {
        FechaVencimiento = fechaVencimiento;
    }

    // Constructor vacío para JSON
    public TareaConVencimiento() : base() { }

    public override void MostrarInfo()
    {
        base.MostrarInfo();
        string estadoVencimiento = DiasRestantes < 0 
            ? $"¡VENCIDA hace {Math.Abs(DiasRestantes)} días!" 
            : $"Días restantes: {DiasRestantes}";
        
        Console.WriteLine($"   Vence: {FechaVencimiento:dd/MM/yyyy} ({estadoVencimiento})");
    }
}