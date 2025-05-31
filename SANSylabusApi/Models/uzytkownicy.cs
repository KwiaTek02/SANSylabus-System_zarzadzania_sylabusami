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

    public virtual ICollection<koordynatorzy_sylabusu> koordynatorzy_sylabusus { get; set; } = new List<koordynatorzy_sylabusu>();

    public virtual ICollection<sylabus_historium> sylabus_historia { get; set; } = new List<sylabus_historium>();

    public virtual ICollection<sylabusy> sylabusies { get; set; } = new List<sylabusy>();
}
