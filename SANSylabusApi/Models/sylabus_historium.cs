using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class sylabus_historium
{
    public int id { get; set; }

    public int sylabus_id { get; set; }

    public DateOnly data_zmiany { get; set; }

    public DateTime czas_zmiany { get; set; }

    // Klucz obcy – ID użytkownika, który wprowadził zmianę
    public int zmieniony_przez { get; set; }

    // Opcjonalny opis zmiany (np. „Zmieniono treści kształcenia”)
    public string? opis_zmiany { get; set; }

    // Opcjonalnie: wersja sylabusu w momencie zmiany (np. „1.2”)
    public string? wersja_wtedy { get; set; }

    // Nawigacja – sylabus, którego dotyczy zmiana
    public virtual sylabusy sylabus { get; set; } = null!;

    // Nawigacja – użytkownik, który dokonał zmiany
    public virtual uzytkownicy zmieniony_przezNavigation { get; set; } = null!;
}
