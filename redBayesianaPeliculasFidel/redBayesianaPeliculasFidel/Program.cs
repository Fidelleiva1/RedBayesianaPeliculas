using System;
using System.Collections.Generic;
using System.Linq;

class RedBayesianaPeliculas
{
    private Dictionary<string, double> probabilidadGenero;
    private Dictionary<string, Dictionary<string, Dictionary<string, double>>> probabilidadAtributosDadoGenero;

    public RedBayesianaPeliculas()
    {
        probabilidadGenero = new Dictionary<string, double>
        {
            { "Comedia", 0.25 },
            { "Drama", 0.20 },
            { "Accion", 0.15 },
            { "Romantica", 0.10 },
            { "Ciencia Ficcion", 0.10 },
            { "Documental", 0.10 },
            { "Terror", 0.10 }
        };

        probabilidadAtributosDadoGenero = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>
        {
            {
                "Duracion", new Dictionary<string, Dictionary<string, double>>
                {
                    { "Comedia", new Dictionary<string, double> { { "Corta", 0.2 }, { "Media", 0.6 }, { "Larga", 0.2 } } },
                    { "Drama", new Dictionary<string, double> { { "Corta", 0.1 }, { "Media", 0.4 }, { "Larga", 0.5 } } },
                    { "Accion", new Dictionary<string, double> { { "Corta", 0.1 }, { "Media", 0.4 }, { "Larga", 0.5 } } },
                    { "Romantica", new Dictionary<string, double> { { "Corta", 0.2 }, { "Media", 0.6 }, { "Larga", 0.2 } } },
                    { "Documental", new Dictionary<string, double> { { "Corta", 0.1 }, { "Media", 0.3 }, { "Larga", 0.6 } } },
                    { "Terror", new Dictionary<string, double> { { "Corta", 0.3 }, { "Media", 0.5 }, { "Larga", 0.2 } } },
                    { "Ciencia Ficcion", new Dictionary<string, double> { { "Corta", 0.1 }, { "Media", 0.3 }, { "Larga", 0.6 } } }
                }
            },
            {
                "Estilo", new Dictionary<string, Dictionary<string, double>>
                {
                    { "Comedia", new Dictionary<string, double> { { "Moderno", 0.5 }, { "Clasica", 0.5 } } },
                    { "Drama", new Dictionary<string, double> { { "Moderno", 0.5 }, { "Clasica", 0.5 } } },
                    { "Accion", new Dictionary<string, double> { { "Moderno", 0.8 }, { "Clasica", 0.2 } } },
                    { "Romantica", new Dictionary<string, double> { { "Moderno", 0.5 }, { "Clasica", 0.5 } } },
                    { "Documental", new Dictionary<string, double> { { "Moderno", 0.8 }, { "Clasica", 0.2 } } },
                    { "Terror", new Dictionary<string, double> { { "Moderno", 0.7 }, { "Clasica", 0.3 } } },
                    { "Ciencia Ficcion", new Dictionary<string, double> { { "Moderno", 0.6 }, { "Clasica", 0.4 } } }
                }
            }
        };
    }

    public double InferirProbabilidadGenero(string genero, string duracion, string estilo)
    {
        double probGenero = probabilidadGenero[genero];
        double probDuracion = probabilidadAtributosDadoGenero["Duracion"][genero][duracion];
        double probEstilo = probabilidadAtributosDadoGenero["Estilo"][genero][estilo];

        double probConjunta = probGenero * probDuracion * probEstilo;
        return probConjunta;
    }

    public (string, Dictionary<string, double>) Diagnostico(string duracion, string estilo)
    {
        Dictionary<string, double> probabilidades = new Dictionary<string, double>();

        foreach (var genero in probabilidadGenero.Keys)
        {
            probabilidades[genero] = InferirProbabilidadGenero(genero, duracion, estilo);
        }

        double totalProb = probabilidades.Values.Sum();
        foreach (var genero in probabilidadGenero.Keys.ToList())
        {
            probabilidades[genero] /= totalProb;
        }

        string generoRecomendado = probabilidades.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        return (generoRecomendado, probabilidades);
    }
}

class Program
{
    static void Main(string[] args)
    {
        RedBayesianaPeliculas red = new RedBayesianaPeliculas();

        Console.WriteLine("¡Bienvenido al sistema experto de recomendación de peliculas hecho por Fidel Leiva!");

        Console.Write("¿Prefieres una pelicula corta, media o larga?: ");
        string duracion = Console.ReadLine().Trim().ToLower();

        Console.Write("¿Qué estilo prefieres? (Moderno, Clasica): ");
        string estilo = Console.ReadLine().Trim().ToLower();

        var resultado = red.Diagnostico(CapitalizarPrimeraLetra(duracion), CapitalizarPrimeraLetra(estilo));

        string generoRecomendado = resultado.Item1;
        var probabilidades = resultado.Item2;

        Console.WriteLine($"Te recomendamos ver la pelicula del género: {generoRecomendado}");
        Console.WriteLine("Probabilidades por género:");
        foreach (var prob in probabilidades)
        {
            Console.WriteLine($"{prob.Key}: {prob.Value * 100:F2}%");
        }
    }

    static string CapitalizarPrimeraLetra(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return char.ToUpper(input[0]) + input.Substring(1);
    }
}
