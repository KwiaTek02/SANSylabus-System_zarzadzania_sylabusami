using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class siatka_przedmiotow
{
    public int id { get; set; }

    public int przedmiot_id { get; set; }

    public string typ { get; set; } = null!;

    public int? wyklad { get; set; }

    public int? cwiczenia { get; set; }

    public int? konwersatorium { get; set; }

    public int? laboratorium { get; set; }

    public int? warsztaty { get; set; }

    public int? projekt { get; set; }

    public int? seminarium { get; set; }

    public int? konsultacje { get; set; }

    public int? egzaminy { get; set; }

    // Suma wszystkich godzin (może być obliczana lub wprowadzana ręcznie)
    public int? sumagodzin { get; set; }

    // Nawigacja – przedmiot, którego dotyczy ta siatka
    public virtual przedmioty przedmiot { get; set; } = null!;
}
