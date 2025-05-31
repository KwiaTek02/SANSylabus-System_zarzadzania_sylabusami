using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class sylabus_historium
{
    public int id { get; set; }

    public int sylabus_id { get; set; }

    public DateOnly data_zmiany { get; set; }

    public DateTime czas_zmiany { get; set; }

    public int zmieniony_przez { get; set; }

    public string? opis_zmiany { get; set; }

    public string? wersja_wtedy { get; set; }

    public virtual sylabusy sylabus { get; set; } = null!;

    public virtual uzytkownicy zmieniony_przezNavigation { get; set; } = null!;
}
