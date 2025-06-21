using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class przedmioty
{
    public int id { get; set; }

    public string nazwa { get; set; } = null!;

    public string? osrodek { get; set; }

    public byte? semestr { get; set; }

    public string? stopien { get; set; }

    public string? kierunek { get; set; }

    public int? suma_godzin_calosciowe { get; set; }

    // Nawigacja: kolekcja siatek zajęć przypisanych do tego przedmiotu
    public virtual ICollection<siatka_przedmiotow> siatka_przedmiotows { get; set; } = new List<siatka_przedmiotow>();

    // Nawigacja: kolekcja sylabusów, które dotyczą tego przedmiotu
    public virtual ICollection<sylabusy> sylabusies { get; set; } = new List<sylabusy>();
}
