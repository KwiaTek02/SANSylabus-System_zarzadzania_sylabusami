using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class uzytkownicy
{
    public int id { get; set; }

    public string imie_nazwisko { get; set; } = null!;

    public string? tytul { get; set; }

    public string login { get; set; } = null!;

    public string haslo { get; set; } = null!;

    public string sol { get; set; } = null!;

    public string email { get; set; } = null!;

    public string typ_konta { get; set; } = null!;

    // Lista powiązań: użytkownik jako koordynator w sylabusach
    public virtual ICollection<koordynatorzy_sylabusu> koordynatorzy_sylabusus { get; set; } = new List<koordynatorzy_sylabusu>();

    // Lista zmian sylabusów dokonanych przez użytkownika (historia edycji)
    public virtual ICollection<sylabus_historium> sylabus_historia { get; set; } = new List<sylabus_historium>();

    // Lista sylabusów utworzonych przez użytkownika
    public virtual ICollection<sylabusy> sylabusies { get; set; } = new List<sylabusy>();
}
