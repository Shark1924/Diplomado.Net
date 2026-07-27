namespace ejercicio_02_poo;

class Program
{
    private const string ARCHIVO_JSON = "tareas.json";

    static void Main(string[] args)
    {
        GestorTareas gestor = new GestorTareas();
        gestor.CargarDeJSON(ARCHIVO_JSON);

        bool salir = false;

        while (!salir)
        {
            Console.WriteLine("\n=== GESTOR DE TAREAS ===");
            Console.WriteLine("1. Agregar tarea");
            Console.WriteLine("2. Listar todas");
            Console.WriteLine("3. Listar por categoría");
            Console.WriteLine("4. Listar por prioridad");
            Console.WriteLine("5. Marcar como completada");
            Console.WriteLine("6. Mostrar tareas vencidas");
            Console.WriteLine("7. Eliminar tarea");
            Console.WriteLine("8. Exportar a JSON");
            Console.WriteLine("9. Salir");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    AgregarTareaMenu(gestor);
                    break;
                case "2":
                    ListarPolimorfico(gestor.ObtenerTodas());
                    break;
                case "3":
                    Console.Write("Ingrese la categoría: ");
                    string cat = Console.ReadLine() ?? "";
                    ListarPolimorfico(gestor.ListarPorCategoria(cat));
                    break;
                case "4":
                    Console.WriteLine("Prioridades: 0.Baja, 1.Media, 2.Alta, 3.Critica");
                    Console.Write("Seleccione número: ");
                    if (int.TryParse(Console.ReadLine(), out int prioInt) && Enum.IsDefined(typeof(Prioridad), prioInt))
                    {
                        ListarPolimorfico(gestor.ListarPorPrioridad((Prioridad)prioInt));
                    }
                    else
                    {
                        Console.WriteLine("Prioridad inválida.");
                    }
                    break;
                case "5":
                    Console.Write("ID de la tarea a completar: ");
                    if (int.TryParse(Console.ReadLine(), out int idComp)) gestor.Completar(idComp);
                    break;
                case "6":
                    Console.WriteLine("\n--- TAREAS VENCIDAS ---");
                    ListarPolimorfico(gestor.ObtenerVencidas());
                    break;
                case "7":
                    Console.Write("ID de la tarea a eliminar: ");
                    if (int.TryParse(Console.ReadLine(), out int idElim)) gestor.Eliminar(idElim);
                    break;
                case "8":
                    gestor.GuardarEnJSON(ARCHIVO_JSON);
                    Console.WriteLine($"✓ Tareas guardadas manualmente en {ARCHIVO_JSON}");
                    break;
                case "9":
                    gestor.GuardarEnJSON(ARCHIVO_JSON);
                    Console.WriteLine("Guardando cambios... ¡Hasta luego!");
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    // Demostración explícita de POLIMORFISMO
    static void ListarPolimorfico(List<Tarea> lista)
    {
        if (lista.Count == 0)
        {
            Console.WriteLine("No se encontraron tareas.");
            return;
        }

        Console.WriteLine("\n-------------------------------------------");
        foreach (Tarea t in lista)
        {
            // Ejecución polimórfica: llama a Tarea.MostrarInfo() o TareaConVencimiento.MostrarInfo() segun corresponda
            t.MostrarInfo();
            Console.WriteLine($"   Exportable: {t.Exportar()}");
            Console.WriteLine("-------------------------------------------");
        }
    }

    static void AgregarTareaMenu(GestorTareas gestor)
    {
        Console.Write("Título: ");
        string titulo = Console.ReadLine() ?? "";

        Console.Write("Descripción: ");
        string desc = Console.ReadLine() ?? "";

        Console.WriteLine("Prioridad (0=Baja, 1=Media, 2=Alta, 3=Critica): ");
        Enum.TryParse(Console.ReadLine(), out Prioridad prio);

        Console.Write("Categoría: ");
        string cat = Console.ReadLine() ?? "";

        Console.Write("¿Tiene fecha de vencimiento? (s/n): ");
        string resp = Console.ReadLine()?.ToLower() ?? "";

        if (resp == "s")
        {
            Console.Write("Fecha de vencimiento (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime fecha))
            {
                gestor.Agregar(new TareaConVencimiento(titulo, desc, prio, cat, fecha));
                Console.WriteLine("✓ Tarea con vencimiento agregada.");
            }
            else
            {
                Console.WriteLine("Fecha inválida.");
            }
        }
        else
        {
            gestor.Agregar(new Tarea(titulo, desc, prio, cat));
            Console.WriteLine("✓ Tarea simple agregada.");
        }
    }
}