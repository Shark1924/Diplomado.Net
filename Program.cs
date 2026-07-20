using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace EvaluadorFinanciero
{
    class Program
    {
        // Contadores globales con nombres independientes para el reporte estadístico
        static int registrosAprobados = 0;
        static int registrosRechazados = 0;
        static Dictionary<string, int> conteoFranquicias = new Dictionary<string, int>()
        {
            { "Visa", 0 }, { "Mastercard", 0 }, { "American Express", 0 }, { "Discover", 0 }, { "Desconocida", 0 }
        };

        static void Main(string[] args)
        {
            int seleccionUsuario;
            do
            {
                Console.Clear();
                // MENÚ ORIGINAL SOLICITADO POR EL PROFESOR
                Console.WriteLine("=== VALIDADOR DE TARJETAS ===");
                Console.WriteLine("1. Validar una tarjeta");
                Console.WriteLine("2. Validar desde archivo");
                Console.WriteLine("3. Generar número válido");
                Console.WriteLine("4. Estadísticas");
                Console.WriteLine("5. Salir");
                Console.Write("\nSeleccione una opción: ");

                if (!int.TryParse(Console.ReadLine(), out seleccionUsuario))
                {
                    seleccionUsuario = 0; // Reinicia si el input no es numérico
                }

                try
                {
                    ControladorFlujo(seleccionUsuario);
                }
                catch (Exception error)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"\n[ALERTA SISTEMA]: Excepción detectada -> {error.Message}");
                    Console.ResetColor();
                    EsperarConfirmacion();
                }

            } while (seleccionUsuario != 5);
        }

        static void ControladorFlujo(int seleccion)
        {
            switch (seleccion)
            {
                case 1:
                    Console.Write("\nIngrese el número de tarjeta (solo números): ");
                    // Captura segura contra valores nulos usando ?? ""
                    string cadenaTarjeta = (Console.ReadLine() ?? "").Replace(" ", "").Trim();
                    EjecutarAnalisis(cadenaTarjeta);
                    break;

                case 2:
                    Console.Write("\nIngrese la ruta del archivo de texto (.txt): ");
                    string ubicacionArchivo = (Console.ReadLine() ?? "").Trim('"');
                    AuditarArchivoTexto(ubicacionArchivo);
                    break;

                case 3:
                    string tokenGenerado = FabricarTokenLuhn();
                    string tipoFranquicia = DetectarFranquicia(tokenGenerado);
                    Console.WriteLine($"\nNúmero Generado: {tokenGenerado}");
                    Console.WriteLine($"Marca: {tipoFranquicia}");
                    Console.WriteLine("Estado: ✅ VÁLIDA (Verificada por algoritmo de Luhn)");
                    break;

                case 4:
                    DesplegarMétricas();
                    break;

                case 5:
                    Console.WriteLine("\n¡Gracias por usar el validador de tarjetas! Saliendo...");
                    break;

                default:
                    Console.WriteLine("\n❌ Opción inválida. Intente de nuevo.");
                    break;
            }

            if (seleccion != 5) EsperarConfirmacion();
        }

        // === 1. ALGORITMO DE LUHN (Variación de ciclo indexado por resta) ===
        static bool ChequearFormulaLuhn(string digitos)
        {
            if (string.IsNullOrWhiteSpace(digitos) || !digitos.All(char.IsDigit) || digitos.Length < 13 || digitos.Length > 19)
                return false;

            int acumuladorPuntaje = 0;
            int longitudCadena = digitos.Length;

            for (int posicion = 0; posicion < longitudCadena; posicion++)
            {
                int indiceInverso = longitudCadena - 1 - posicion;
                int valorActual = (int)char.GetNumericValue(digitos[indiceInverso]);

                if (posicion % 2 == 1)
                {
                    valorActual *= 2;
                    if (valorActual > 9)
                    {
                        valorActual = (valorActual / 10) + (valorActual % 10);
                    }
                }

                acumuladorPuntaje += valorActual;
            }

            return (acumuladorPuntaje % 10 == 0);
        }

        // === 2. DETECCIÓN DE FRANQUICIAS ===
        static string DetectarFranquicia(string digitos)
        {
            if (string.IsNullOrWhiteSpace(digitos) || !digitos.All(char.IsDigit)) return "Desconocida";

            int tamano = digitos.Length;

            if (digitos.StartsWith("4") && (tamano == 13 || tamano == 16))
                return "Visa";

            if (tamano == 16 && int.TryParse(digitos.Substring(0, 2), out int prefijoDos) && prefijoDos >= 51 && prefijoDos <= 55)
                return "Mastercard";

            if (tamano == 15 && (digitos.StartsWith("34") || digitos.StartsWith("37")))
                return "American Express";

            if (tamano >= 16 && tamano <= 19)
            {
                if (digitos.StartsWith("6011") || digitos.StartsWith("644") || digitos.StartsWith("645") ||
                    digitos.StartsWith("646") || digitos.StartsWith("647") || digitos.StartsWith("648") ||
                    digitos.StartsWith("649") || digitos.StartsWith("65"))
                    return "Discover";

                if (int.TryParse(digitos.Substring(0, 6), out int prefijoSeis) && prefijoSeis >= 622126 && prefijoSeis <= 622925)
                    return "Discover";
            }

            return "Desconocida";
        }

        // === 3. ANÁLISIS INDIVIDUAL ===
        static void EjecutarAnalisis(string digitos)
        {
            string redAsociada = DetectarFranquicia(digitos);
            bool resultadoValidacion = ChequearFormulaLuhn(digitos);

            Console.WriteLine($"\nNúmero: {digitos}");
            Console.WriteLine($"Marca: {redAsociada}");

            if (resultadoValidacion)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Estado: ✅ VÁLIDA");
                registrosAprobados++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Estado: ❌ INVÁLIDA");
                registrosRechazados++;
            }
            Console.ResetColor();

            conteoFranquicias[redAsociada]++;
        }

        // === 4. PROCESAMIENTO EN LOTES ===
        static void AuditarArchivoTexto(string ruta)
        {
            if (!File.Exists(ruta))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ El archivo especificado no existe. Verifique la ruta.");
                Console.ResetColor();
                return;
            }

            try
            {
                string[] todasLasLineas = File.ReadAllLines(ruta);
                int contadorInterno = 0;

                Console.WriteLine("\n--- Procesando Archivo ---");
                foreach (string fila in todasLasLineas)
                {
                    string textoLimpio = fila.Replace(" ", "").Trim();
                    if (!string.IsNullOrEmpty(textoLimpio))
                    {
                        EjecutarAnalisis(textoLimpio);
                        contadorInterno++;
                        Console.WriteLine(new string('-', 30));
                    }
                }

                Console.WriteLine($"\nResumen: Se procesaron exitosamente {contadorInterno} líneas del archivo.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"❌ Error al leer el archivo: {ex.Message}");
            }
        }

        // === 5. GENERACIÓN CON FUERZA BRUTA CONTROLADA ===
        static string FabricarTokenLuhn()
        {
            Random aleatorio = new Random();
            string semilla = "4"; // Usamos prefijo de Visa para asegurar una estructura estándar
            
            for (int k = 0; k < 14; k++)
            {
                semilla += aleatorio.Next(0, 10).ToString();
            }

            for (int digitoPrueba = 0; digitoPrueba <= 9; digitoPrueba++)
            {
                string pruebaCompleta = semilla + digitoPrueba;
                if (ChequearFormulaLuhn(pruebaCompleta))
                {
                    return pruebaCompleta;
                }
            }

            return semilla + "0";
        }

        // === 6. DESPLIEGUE DE ESTADÍSTICAS ===
        static void DesplegarMétricas()
        {
            Console.WriteLine("\n=== ESTADÍSTICAS GENERALES ===");
            Console.WriteLine($"Tarjetas Válidas:  {registrosAprobados}");
            Console.WriteLine($"Tarjetas Inválidas: {registrosRechazados}");
            Console.WriteLine($"Total Procesadas:   {registrosAprobados + registrosRechazados}");
            Console.WriteLine("\nDesglose por marca:");
            
            foreach (var elemento in conteoFranquicias)
            {
                Console.WriteLine($" - {elemento.Key}: {elemento.Value}");
            }
        }

        static void EsperarConfirmacion()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}