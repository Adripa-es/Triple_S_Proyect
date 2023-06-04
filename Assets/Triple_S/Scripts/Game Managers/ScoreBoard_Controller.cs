using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class ScoreBoard_Controller : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fechaText;
    [SerializeField] private TextMeshProUGUI tiempoSobrevividoText;
    [SerializeField] private TextMeshProUGUI puntosText;
    [SerializeField] private TextMeshProUGUI monedasText;
    private const string DataSeparator = "||";
    private const int ExpectedDataLength = 4;
    private string dataStats = "PlayerStats.txt";

    private void Start()
    {
        OrdenarEstadisticas();
        LeerDatos();
    }

    // Obtiene la ruta del archivo de estadísticas
    private string GetStatsFilePath()
    {
        return Path.Combine(Application.dataPath, dataStats);
    }

    // Ordena las estadísticas en el archivo
    private void OrdenarEstadisticas()
    {
        string statsFilePath = GetStatsFilePath();

        if (File.Exists(statsFilePath))
        {
            List<string> lines;
            using (StreamReader reader = new StreamReader(statsFilePath))
            {
                lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            lines.Sort((x, y) =>
            {
                string[] xValues = x.Split(new string[] { DataSeparator }, StringSplitOptions.None);
                string[] yValues = y.Split(new string[] { DataSeparator }, StringSplitOptions.None);

                float xSurvivalTime, ySurvivalTime;
                int xPoints, yPoints, xCoins, yCoins;

                float.TryParse(xValues[1], out xSurvivalTime);
                float.TryParse(yValues[1], out ySurvivalTime);

                int.TryParse(xValues[2], out xPoints);
                int.TryParse(yValues[2], out yPoints);
                int.TryParse(xValues[3], out xCoins);
                int.TryParse(yValues[3], out yCoins);

                int result = ySurvivalTime.CompareTo(xSurvivalTime);
                if (result == 0)
                {
                    result = yPoints.CompareTo(xPoints);
                    if (result == 0)
                    {
                        result = yCoins.CompareTo(xCoins);
                    }
                }

                return result;
            });

            int lineCount = Mathf.Min(10, lines.Count);
            lines = lines.GetRange(0, lineCount);

            using (StreamWriter writer = new StreamWriter(statsFilePath))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }
    }

    // Convierte un string con el tiempo de supervivencia a su valor en segundos como float
    private float ParseSurvivalTime(string survivalTimeString)
    {
        string[] timeValues = survivalTimeString.Split(':', ',');
        float hours, minutes, seconds, milliseconds;

        float.TryParse(timeValues[0], out hours);
        float.TryParse(timeValues[1], out minutes);
        float.TryParse(timeValues[2], out seconds);
        float.TryParse(timeValues[3], out milliseconds);

        return hours * 60 * 60 + minutes * 60 + seconds + milliseconds / 1000;
    }

    // Lee los datos de las estadísticas desde el archivo y los muestra en los campos de texto
    private void LeerDatos()
    {
        string statsFilePath = GetStatsFilePath();

        if (File.Exists(statsFilePath))
        {
            string[] lineas;
            using (StreamReader reader = new StreamReader(statsFilePath))
            {
                lineas = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }

            string todasLasFechas = string.Empty;
            string todosLosTiempos = string.Empty;
            string todosLosPuntos = string.Empty;
            string todasLasMonedas = string.Empty;

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(new string[] { DataSeparator }, StringSplitOptions.None);

                if (datos.Length == ExpectedDataLength)
                {
                    todasLasFechas += datos[0] + Environment.NewLine;
                    todosLosTiempos += datos[1] + Environment.NewLine;
                    todosLosPuntos += datos[2] + Environment.NewLine;
                    todasLasMonedas += datos[3] + Environment.NewLine;
                }
                else
                {
                    todasLasFechas += "-" + Environment.NewLine;
                    todosLosTiempos += "-" + Environment.NewLine;
                    todosLosPuntos += "-" + Environment.NewLine;
                    todasLasMonedas += "-" + Environment.NewLine;
                }
            }

            fechaText.text = todasLasFechas;
            tiempoSobrevividoText.text = todosLosTiempos;
            puntosText.text = todosLosPuntos;
            monedasText.text = todasLasMonedas;
        }
        else
        {
            // Mostrar guiones en lugar de un mensaje de error de depuración
            fechaText.text = "-";
            tiempoSobrevividoText.text = "-";
            puntosText.text = "-";
            monedasText.text = "-";
        }
    }
}
